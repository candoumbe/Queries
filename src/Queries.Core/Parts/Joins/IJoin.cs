using Queries.Core.Parts.Clauses;

namespace Queries.Core.Parts.Joins
{
    /// <summary>
    /// Interface of join operators
    /// </summary>
    public interface IJoin
    {
        /// <summary>
        /// The type of JoinOperator
        /// </summary>
        JoinType JoinType { get; }
        /// <summary>
        /// <see cref="Table"/> onto which the join operator will be applied
        /// </summary>
        Table Table { get; }
        /// <summary>
        /// clause to applied to the join element.
        /// </summary>
        IWhereClause On { get; }
    }
}
