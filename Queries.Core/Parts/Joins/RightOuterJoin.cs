using Queries.Core.Parts.Clauses;

namespace Queries.Core.Parts.Joins
{
    public class RightOuterJoin : JoinBase
    {
        public RightOuterJoin(Table table, IWhereClause @on)
            : base(JoinType.RightOuterJoin, table, @on)
        { }
    }
}