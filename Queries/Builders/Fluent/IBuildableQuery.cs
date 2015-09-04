namespace Queries.Builders.Fluent
{
    public interface IBuildableQuery<T>
    {
        T Build();
    }
}
