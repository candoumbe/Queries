namespace Queries.Builders.Fluent
{
    public interface IBuildableQuery<out T>
    {
        T Build();
    }
}
