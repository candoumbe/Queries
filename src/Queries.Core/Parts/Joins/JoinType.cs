namespace Queries.Core.Parts.Joins;

/// <summary>
/// Type of joins
/// </summary>
public enum JoinType
{
    /// <summary>
    /// Cartesian join
    /// </summary>
    CrossJoin,

    /// <summary>
    /// Left outer join
    /// </summary>
    LeftOuterJoin,

    /// <summary>
    /// Right outer join
    /// </summary>
    RightOuterJoin,

    /// <summary>
    /// Inner join
    /// </summary>
    InnerJoin
}