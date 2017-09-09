using Queries.Core.Parts;

namespace Queries.Core.Builders.Fluent
{
    /// <summary>
    /// Marks an element so that <see cref="Union(IUnionQuery{T})"/> can be called.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUnionQuery<T> : IAliasable<ITable>, IBuildableQuery<T>, ITable
    {
        /// <summary>
        /// Called union between the current instance and <see cref="query"/>.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IUnionQuery<T> Union(IUnionQuery<T> query);
    }
}
