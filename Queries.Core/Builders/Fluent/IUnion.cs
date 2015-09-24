namespace Queries.Core.Builders.Fluent
{
    public interface IUnionQuery<T> : IBuildableQuery<T>
    {
        IUnionQuery<T> Union(IUnionQuery<T> query);
    }
}
