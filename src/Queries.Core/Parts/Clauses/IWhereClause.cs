using System;

namespace Queries.Core.Parts.Clauses;

/// <summary>
/// Marks a class so that it can be used in a "WHERE" clause.
/// </summary>
public interface IWhereClause : IEquatable<IWhereClause>
{
    /// <summary>
    /// Performs deep cloning of the current instance.
    /// </summary>
    /// <returns></returns>
    IWhereClause Clone();
}