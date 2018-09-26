using Queries.Core.Parts;

namespace Queries.Core.Builders.Fluent
{
    /// <summary>
    /// Defines the shape of a SELECT query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISelectQuery<T>
    {
        ISelectQuery<T> Limit(int limit);

        IFromQuery<T> From(params ITable[] tables);

        IFromQuery<T> From(params string[] tables);
    }
}