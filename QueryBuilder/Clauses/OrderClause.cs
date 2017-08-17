namespace SqlKata
{
    public abstract class AbstractOrderBy : AbstractClause
    {
    }

    public class OrderBy : AbstractOrderBy
    {
        public string Column { get; set; }
        public bool Ascending { get; set; } = true;

        public override AbstractClause Clone()
        {
            return new OrderBy
            {
                Engine = Engine,
                EngineVersion = EngineVersion,
                Component = Component,
                Column = Column,
                Ascending = Ascending
            };
        }
    }

    public class RawOrderBy : AbstractOrderBy, RawInterface
    {
        protected object[] _bindings;
        public string Expression { get; set; }
        public object[] Bindings { set => _bindings = value; }

        public override object[] GetBindings(string engine, int engineVersion)
        {
            return _bindings;
        }

        public override AbstractClause Clone()
        {
            return new RawOrderBy
            {
                Engine = Engine,
                EngineVersion = EngineVersion,
                Component = Component,
                Expression = Expression,
                _bindings = _bindings
            };
        }
    }

    public class OrderByRandom : AbstractOrderBy
    {
        public override AbstractClause Clone()
        {
            return new OrderByRandom
            {
                Engine = Engine,
                EngineVersion = EngineVersion
            };
        }
    }
}