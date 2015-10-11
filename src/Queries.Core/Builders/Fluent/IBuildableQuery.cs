namespace Queries.Core.Builders.Fluent
{
    public interface IBuildableQuery<out T>
    {
        T Build();
    }
}
