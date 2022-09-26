using Queries.Core.Parts;

namespace Queries.Core.Builders.Fluent;

/// <summary>
/// Defines the shape of a SELECT query
/// </summary>
/// <typeparam name="TQuery">Type of the query that is being built.</typeparam>
public interface ISelectQuery<TQuery> : IPaginatedQuery<TQuery>
    where TQuery: IQuery
{
    /// <summary>
    /// Applies the <c>FROM</c> instruction to the <typeparamref name="TQuery"/> under construction
    /// </summary>
    /// <param name="tables"></param>
    /// <returns><see cref="IFromQuery{T}"/></returns>
    IFromQuery<TQuery> From(params ITable[] tables);

    /// <summary>
    /// Applies the <c>FROM</c> instruction to the <typeparamref name="TQuery"/> under construction
    /// </summary>
    /// <param name="tables"></param>
    /// <returns><see cref="IFromQuery{T}"/></returns>
    IFromQuery<TQuery> From(params string[] tables);
}