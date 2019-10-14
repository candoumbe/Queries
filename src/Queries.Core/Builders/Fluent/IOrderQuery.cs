namespace Queries.Core.Builders.Fluent
{
    /// <summary>
    /// A marker interface to indicate that the fluent builder can be chained with an <see cref="Parts.Sorting.IOrder"/> instance
    /// </summary>
    /// <typeparam name="T">Type of the query that will be created when calling <see cref="IBuild{TQuery}.Build"/>.</typeparam>
    public interface IOrderQuery<out T> : IPaginatedQuery<T>
    {
    }
}
