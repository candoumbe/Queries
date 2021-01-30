namespace Queries.Core.Builders.Fluent
{
    /// <summary>
    /// Interface for fluent builders.
    /// </summary>
    /// <typeparam name="TQuery">Type of query that the current instance will build</typeparam>
    public interface IBuild<out TQuery> : IQuery
    {
        /// <summary>
        /// Builds the <see cref="TQuery"/> element
        /// </summary>
        /// <returns><see cref="TQuery"/> instance</returns>
        TQuery Build();
    }
}
