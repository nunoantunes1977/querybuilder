namespace SqlKata
{
    public abstract partial class BaseQuery<Q>
    {
        public Q WhereFtsContains(string column, string searchTerm, string searchCondition = "AND")
        {
            return Add("where", new FtsCondition
            {
                Column = column,
                SearchTerm = searchTerm,
                SearchCondition = searchCondition,
                IsOr = getOr(),
                IsNot = getNot(),
            });
        }

        public Q WhereFtsContainsIf(bool condition, string column, string searchTerm, string searchCondition = "AND")
        {
            if (condition)
            {
                return WhereFtsContains(column, searchTerm, searchCondition);
            }

            return (Q)this;
        }
    }
}