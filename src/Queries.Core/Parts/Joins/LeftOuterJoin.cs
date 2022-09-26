using Queries.Core.Parts.Clauses;

namespace Queries.Core.Parts.Joins;

/// <summary>
/// Models a LEFT OUTER Join
/// </summary>
public class LeftOuterJoin : JoinBase
{
    /// <summary>
    /// Builds a new <see cref="LeftOuterJoin"/> instance.
    /// </summary>
    /// <param name="table"><see cref="Table"/> the join will be applied to</param>
    /// <param name="on">Joint criteria</param>
    public LeftOuterJoin(Table table, IWhereClause @on)
        : base(JoinType.LeftOuterJoin, table, @on)
    { }
}