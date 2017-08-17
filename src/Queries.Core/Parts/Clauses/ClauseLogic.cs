namespace Queries.Core.Parts.Clauses
{
    /// <summary>
    /// Logical operator to use when combining multiple <see cref="IWhereClause"/>/ (resp. <see cref="IHavingClause"/>) 
    /// instances using <see cref="CompositeWhereClause"/> (resp. <see cref="CompositeHavingClause"/>).
    /// </summary>
    public enum ClauseLogic
    {
        And = 0,
        Or = 2
    }
}