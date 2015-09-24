using Queries.Core.Parts.Clauses;

namespace Queries.Core.Parts.Joins
{
    public class LeftOuterJoin : JoinBase
    {
        public LeftOuterJoin(Table table, IWhereClause @on)
            : base(JoinType.LeftOuterJoin, table, @on)
        { }
    }
}