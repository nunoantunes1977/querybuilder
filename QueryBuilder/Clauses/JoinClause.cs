using System;

namespace SqlKata
{
    public abstract class AbstractJoin : AbstractClause
    {
    }

    public class BaseJoin : AbstractJoin
    {
        public Join Join { get; set; }

        public override object[] GetBindings(string engine, int engineVersion)
        {
            return Join.GetBindings(engine, engineVersion).ToArray();
        }

        public override AbstractClause Clone()
        {
            return new BaseJoin
            {
                Engine = Engine,
                EngineVersion = EngineVersion,
                Join = Join.Clone(),
                Component = Component,
            };
        }
    }

    public class DeepJoin : AbstractJoin
    {
        public string Type { get; set; }
        public string Expression { get; set; }
        public string SourceKeySuffix { get; set; }
        public string TargetKey { get; set; }
        public Func<string, string> SourceKeyGenerator { get; set; }
        public Func<string, string> TargetKeyGenerator { get; set; }

        public override AbstractClause Clone()
        {
            return new DeepJoin
            {
                Engine = Engine,
                EngineVersion = EngineVersion,
                Component = Component,
                Type = Type,
                Expression = Expression,
                SourceKeySuffix = SourceKeySuffix,
                TargetKey = TargetKey,
                SourceKeyGenerator = SourceKeyGenerator,
                TargetKeyGenerator = TargetKeyGenerator,
            };
        }
    }
}