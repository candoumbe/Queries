using Queries.Core.Parts.Clauses;

namespace Queries.Core.Parts.Joins;

/// <summary>
/// Represents a "INNER JOIN"
/// </summary>
public class InnerJoin : JoinBase
{
    /// <summary>
    /// Builds a new <see cref="InnerJoin"/> instance.
    /// </summary>
    /// <param name="table"></param>
    /// <param name="on"></param>
    public InnerJoin(Table table, IWhereClause @on) : base(JoinType.InnerJoin, table, @on)
    {}
}