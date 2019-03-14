using Queries.Core.Parts.Sorting;

namespace Queries.Core.Builders.Fluent
{
    /// <summary>
    /// Interface to chain 
    /// </summary>
    /// <typeparam name="T">Type of the query under construction</typeparam>
    public interface IWhereQuery<T> : IHavingQuery<T>, IInsertable
    {
        /// <summary>
        /// Next available operation
        /// </summary>
        /// <param name="sort">Sort operation</param>
        /// <param name="sorts">Additional sort operations</param>
        /// <returns><see cref="ISortQuery{T}"/></returns>
        ISortQuery<T> OrderBy(ISort sort, params ISort[] sorts);
    }
}