using Queries.Parts.Sorting;

namespace Queries.Builders.Fluent
{
    public interface IWhereQuery<T> : IBuildableQuery<T>
    {

        ISortQuery<T> OrderBy(params ISort[] sorts);

    }
}