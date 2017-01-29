namespace Queries.Core.Builders.Fluent
{
    public interface IBuildableQuery<out T> : IQuery
    {
        T Build();
    }
}
