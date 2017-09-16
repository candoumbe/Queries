namespace Queries.Core.Builders.Fluent
{
    /// <summary>
    /// Interface for fluent builders.
    /// </summary>
    /// <typeparam name="T">Type of element that can be build</typeparam>
    public interface IBuild<out T> : IQuery
    {
        /// <summary>
        /// Builds the <see cref="T"/> element
        /// </summary>
        /// <returns><see cref="T"/> instance</returns>
        T Build();
    }
}
