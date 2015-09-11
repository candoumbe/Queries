using Queries.Parts.Clauses;

namespace Queries.Parts.Joins
{
    public class LeftOuterJoin : JoinBase
    {
        public LeftOuterJoin(Table table, IWhereClause @on)
            : base(JoinType.LeftOuterJoin, table, @on)
        { }
    }
}