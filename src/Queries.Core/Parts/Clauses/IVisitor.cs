namespace Queries.Core.Parts.Clauses;

/// <summary>
/// Inteface to implement when visiting a <typeparammref name="TQuery"/> instance.
/// </summary>
/// <typeparam name="TQuery">Type of the visited element.</typeparam>
public interface IVisitor<TQuery>
{
    /// <summary>
    /// Visits the specified <typeparamref name="TQuery"/>  <paramref name="instance"/>.
    /// </summary>
    /// <param name="instance"></param>
    void Visit(TQuery instance);
}
