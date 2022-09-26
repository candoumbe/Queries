namespace Queries.Core.Parts.Joins;

/// <summary>
/// Models a cartesian join
/// </summary>
public class CrossJoin : JoinBase
{
    /// <summary>
    /// Builds a new <see cref="CrossJoin"/> instance.
    /// </summary>
    /// <param name="table"></param>
    public CrossJoin(Table table)
        : base(JoinType.CrossJoin, table, null)
    { }
}