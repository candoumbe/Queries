using Queries.Core.Parts;

namespace Queries.Core.Builders.Fluent
{
    /// <summary>
    /// Marks an element so that <see cref="Union(IUnionQuery{T})"/> can be called.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUnionQuery<T> : IAliasable<ITable>, ITable, IPaginatedQuery<T>
    {
        /// <summary>
        /// Calls UNION between the current instance and <paramref name="query"/>.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IUnionQuery<T> Union(IUnionQuery<T> query);
    }
}
