using Queries.Core.Parts.Columns;

namespace Queries.Core.Builders.Fluent
{
    public interface IPaginatedQuery<out T> : IBuild<T>
    {
        T Paginate(int pageIndex, int pageSize);
    }
}