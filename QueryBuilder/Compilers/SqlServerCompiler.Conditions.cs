namespace SqlKata.Compilers
{
    public partial class SqlServerCompiler : Compiler
    {
        protected override string CompileFtsCondition(FtsCondition x)
        {
            var column = Wrap(x.Column);

            var sql = "CONTAINS((" + column + "), " + Parameter(x.SearchTerm) + ")";

            return sql;
        }
    }
}