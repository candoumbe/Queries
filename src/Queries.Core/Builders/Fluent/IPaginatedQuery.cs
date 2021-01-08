using Queries.Core.Parts.Columns;

namespace Queries.Core.Builders.Fluent
{
    public interface IPaginatedQuery<out T> : IBuild<T>
    {
        /// <summary>
        /// Adds pagination parameters
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        T Paginate(int pageIndex, int pageSize);
    }
}