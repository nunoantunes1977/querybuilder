namespace SqlKata
{
    public abstract class AbstractColumn : AbstractClause
    {
    }

    public class Column : AbstractColumn
    {
        public string Name { get; set; }

        public override AbstractClause Clone()
        {
            return new Column
            {
                Engine = Engine,
                EngineVersion = EngineVersion,
                Name = Name,
                Component = Component,
            };
        }
    }

    public class QueryColumn : AbstractColumn
    {
        public Query Query { get; set; }

        public override object[] GetBindings(string engine, int engineVersion)
        {
            return Query.GetBindings(engine, engineVersion).ToArray();
        }

        public override AbstractClause Clone()
        {
            return new QueryColumn
            {
                Engine = Engine,
                EngineVersion = EngineVersion,
                Query = Query.Clone(),
                Component = Component,
            };
        }
    }

    public class RawColumn : AbstractColumn, RawInterface
    {
        public string Expression { get; set; }
        protected object[] _bindings;
        public object[] Bindings { set => _bindings = value; }

        public override object[] GetBindings(string engine, int engineVersion)
        {
            return _bindings;
        }

        public override AbstractClause Clone()
        {
            return new RawColumn
            {
                Engine = Engine,
                EngineVersion = EngineVersion,
                Expression = Expression,
                _bindings = _bindings,
                Component = Component,
            };
        }
    }
}