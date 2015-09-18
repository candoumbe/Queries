using Queries.Parts.Clauses;

namespace Queries.Parts.Joins
{
    public class RightOuterJoin : JoinBase
    {
        public RightOuterJoin(Table table, IWhereClause @on)
            : base(JoinType.RightOuterJoin, table, @on)
        { }
    }
}