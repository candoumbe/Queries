using Queries.Core.Parts;

namespace Queries.Core.Builders.Fluent
{
    public interface IUnionQuery<T> : IAliasable<ITable>, IBuildableQuery<T>, ITable
    {
        IUnionQuery<T> Union(IUnionQuery<T> query);
    }
}
