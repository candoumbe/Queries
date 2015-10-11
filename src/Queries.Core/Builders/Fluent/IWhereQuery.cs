using Queries.Core.Parts.Sorting;

namespace Queries.Core.Builders.Fluent
{
    public interface IWhereQuery<T> : IHavingQuery<T>
    {
        ISortQuery<T> OrderBy(params ISort[] sorts);

    }
}