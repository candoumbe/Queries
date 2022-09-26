using Queries.Core.Parts.Clauses;

namespace Queries.Core.Builders.Fluent;

/// <summary>
/// Allows to append a <see cref="IWhereClause"/> to the query being built
/// </summary>
/// <typeparam name="T">Type of the query under construction</typeparam>
public interface IJoinQuery<T> : IInsertable, IBuild<T>
{
    /// <summary>
    /// Applies the speicified <paramref name="clause"/> to the current <typeparamref name="T"/> instance.
    /// </summary>
    /// <param name="clause"></param>
    /// <returns><see cref="IWhereQuery{T}"/> instance for further processing</returns>
    IWhereQuery<T> Where(IWhereClause clause);
}