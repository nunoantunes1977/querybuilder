using System;
using System.Linq;

namespace SqlKata.Compilers
{
    public partial class SqlServerCompiler : Compiler
    {
        public SqlServerCompiler(SqlServerVersion engineVersion = SqlServerVersion.SQL2008)
        {
            EngineCode = "sqlsrv";
            EngineVersion = (int)engineVersion;
        }

        protected override string OpeningIdentifier()
        {
            return "[";
        }

        protected override string ClosingIdentifier()
        {
            return "]";
        }

        protected override Query OnBeforeSelect(Query query)
        {
            var limitOffset = query.GetOne<LimitOffset>("limit", EngineCode);

            if (limitOffset == null || !limitOffset.HasOffset() || EngineVersion >= (int)SqlServerVersion.SQL2012)
            {
                return query;
            }

            // Surround the original query with a parent query, then restrict the result to the offset provided, see more at https://docs.microsoft.com/en-us/sql/t-sql/functions/row-number-transact-sql

            var rowNumberColName = "row_num";

            var orderStatement = CompileOrders(query) ?? "ORDER BY (SELECT 0)";

            var orderClause = query.Get("order", EngineCode);

            // get a clone without the limit and order
            query.Clear("order");
            query.Clear("limit");
            var subquery = query.Clone();

            subquery.Clear("cte");

            // Now clear other stuff
            query.Clear("select");
            query.Clear("from");
            query.Clear("join");
            query.Clear("where");
            query.Clear("group");
            query.Clear("having");
            query.Clear("union");

            // Transform the query to make it a parent query
            query.Select("*");

            if (!subquery.Has("select", EngineCode))
            {
                subquery.SelectRaw("*");
            }

            //Add an alias name to the subquery
            subquery.As("subquery");
            
            // Add the row_number select, and put back the bindings here if any
            // Add alias to subquery
            subquery.SelectRaw(
                    $"ROW_NUMBER() OVER ({orderStatement}) AS {WrapValue(rowNumberColName)}",
                    orderClause.SelectMany(x => x.GetBindings(EngineCode, EngineVersion))
            ).As("q");

            query.From(subquery);

            if (limitOffset.HasLimit())
            {
                query.WhereBetween(
                    rowNumberColName,
                    limitOffset.Offset + 1,
                    limitOffset.Limit + limitOffset.Offset
                );
            }
            else
            {
                query.Where(rowNumberColName, ">=", limitOffset.Offset + 1);
            }

            limitOffset.Clear();

            return query;
        }

        protected override string CompileColumns(Query query)
        {
            var compiled = base.CompileColumns(query);

            // If there is a limit on the query, but not an offset, we will add the top
            // clause to the query, which serves as a "limit" type clause within the
            // SQL Server system similar to the limit keywords available in MySQL.
            var limitOffset = query.GetOne("limit", EngineCode) as LimitOffset;

            if (limitOffset != null && limitOffset.HasLimit() && !limitOffset.HasOffset())
            {
                // Add a fake raw select to simulate the top bindings
                query.Clauses.Insert(0, new RawColumn
                {
                    Engine = EngineCode,
                    EngineVersion = EngineVersion,
                    Component = "select",
                    Expression = "",
                    Bindings = new object[] { limitOffset.Limit }
                });

                query.Clear("limit");

                return "SELECT TOP (?)" + compiled.Substring(6);
            }

            return compiled;
        }

        public override string CompileLimit(Query query)
        {
            return "";
        }

        public override string CompileOffset(Query query)
        {
            var limitOffset = query.GetOne<LimitOffset>("limit", EngineCode);

            if (limitOffset != null && limitOffset.HasOffset() && EngineVersion >= (int)SqlServerVersion.SQL2012)
            {
                if (limitOffset.HasLimit() && limitOffset.HasOffset())
                {
                    return "OFFSET ? ROWS FETCH NEXT ? ROWS ONLY";
                }
                else if (!limitOffset.HasLimit() && limitOffset.HasOffset())
                {
                    return "OFFSET ? ROWS";
                }
            }

            return "";
        }

        public override string CompileRandom(string seed)
        {
            return "NEWID()";
        }
    }

    public enum SqlServerVersion
    {
        NotApplicable = 0,
        SQL2008 = 100,
        SQL2008R2 = 105,
        SQL2012 = 110,
        SQL2014 = 120,
        SQL2016 = 130
    }

    public static class SqlServerCompilerExtensions
    {
        public static string ENGINE_CODE = "sqlsrv";

        public static Query ForSqlServer(this Query src, Func<Query, Query> fn)
        {
            return src.For(ENGINE_CODE, fn);
        }
    }
}
