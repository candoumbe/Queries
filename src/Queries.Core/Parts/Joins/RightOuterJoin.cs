using Queries.Core.Parts.Clauses;

namespace Queries.Core.Parts.Joins
{
    public class RightOuterJoin : JoinBase
    {
        /// <summary>
        /// Builds a new <see cref="RightOuterJoin"/> instance.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="on"></param>
        public RightOuterJoin(Table table, IWhereClause @on)
            : base(JoinType.RightOuterJoin, table, @on)
        { }
    }
}