using Queries.Core.Parts.Clauses;

namespace Queries.Core.Parts.Joins
{
    public class InnerJoin : JoinBase
    {
        public InnerJoin(Table table, IWhereClause @on) : base(JoinType.InnerJoin, table, @on)
        {}
    }
}