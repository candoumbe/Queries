using Queries.Core.Parts.Clauses;

namespace Queries.Core.Builders.Fluent;

/// <summary>
/// Fluent interface builder for adding <see cref="IHavingClause"/> clause to <typeparamref name="TQuery"/> instance.
/// </summary>
/// <typeparam name="TQuery">Type of the query under construction.</typeparam>
public interface IHavingQuery<TQuery> : IUnionQuery<TQuery>
{
    /// <summary>
    /// Adds a <see cref="IHavingClause"/> to the <typeparamref name="TQuery"/> under construction
    /// </summary>
    /// <param name="clause">The clause to add to the query</param>
    /// <returns><see cref="IOrderQuery{TQuery}"/></returns>
    IOrderQuery<TQuery> Having(IHavingClause clause);
}