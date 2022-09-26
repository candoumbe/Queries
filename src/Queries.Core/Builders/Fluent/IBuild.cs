namespace Queries.Core.Builders.Fluent;

/// <summary>
/// Interface for fluent builders.
/// </summary>
/// <typeparam name="TQuery">Type of query that the current instance will build</typeparam>
public interface IBuild<out TQuery> : IQuery
{
    /// <summary>
    /// Builds the <typeparamref name="TQuery"/> element.
    /// </summary>
    /// <returns><typeparamref name="TQuery"/> instance</returns>
    TQuery Build();
}
