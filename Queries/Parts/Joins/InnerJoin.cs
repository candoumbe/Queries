using Queries.Parts.Clauses;

namespace Queries.Parts.Joins
{
    public class InnerJoin : JoinBase
    {
        public InnerJoin(Table table, IWhereClause @on) : base(JoinType.InnerJoin, table, @on)
        {}
    }
}