using Queries.Core.Parts.Columns;

namespace Queries.Core.Builders.Fluent;

/// <summary>
/// Fluent interface 
/// </summary>
/// <typeparam name="TQuery">Type of the query that the fluent interface help to build</typeparam>
public interface IInsertIntoQuery<TQuery>
{
    /// <summary>
    /// Adds the specified <paramref name="select"/> as values.
    /// </summary>
    /// <param name="select"></param>
    /// <returns><see cref="IBuild{TQuery}"/></returns>
    IBuild<TQuery> Values(SelectQuery select);

    /// <summary>
    /// Adds the specified <paramref name="value"/> as values to the query under construction.
    /// </summary>
    /// <param name="value">The first <see cref="InsertedValue"/> to add</param>
    /// <param name="values">Additional <see cref="InsertedValue"/>s to add</param>
    /// <returns><see cref="IBuild{TQuery}"/></returns>
    IBuild<TQuery> Values(InsertedValue value, params InsertedValue[] values);
}
