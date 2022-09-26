namespace Queries.Core.Parts.Clauses;

/// <summary>
/// Logical operator to use when combining several <see cref="IWhereClause"/>/ (resp. <see cref="IHavingClause"/>) 
/// instances using <see cref="CompositeWhereClause"/> (resp. <see cref="CompositeHavingClause"/>).
/// </summary>
public enum ClauseLogic
{
    /// <summary>
    /// And logic
    /// </summary>
    And = 0,

    /// <summary>
    /// Or logic
    /// </summary>
    Or = 2
}