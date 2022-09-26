namespace Queries.Core.Parts.Clauses;

/// <summary>
/// A criterion that can be applied onto column which a 
/// </summary>
public interface IHavingClause
{
    /// <summary>
    /// Performs a deep copy of the current instance.
    /// </summary>
    /// <returns></returns>
    IHavingClause Clone();
}