namespace Queries.Core.Builders.Fluent
{
    /// <summary>
    /// Interface for Query builders
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBuildableQuery<out T> : IQuery
    {
        /// <summary>
        /// Builds the query
        /// </summary>
        /// <returns></returns>
        T Build();
    }
}
