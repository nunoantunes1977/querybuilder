namespace SqlKata
{
    public abstract class AbstractClause
    {
        public string Engine { get; set; } = null;
        public int EngineVersion { get; set; } = 0;
        public string Component { get; set; }

        public virtual object[] GetBindings(string engine, int engineVersion)
        {
            return new object[] { };
        }

        public abstract AbstractClause Clone();
    }
}