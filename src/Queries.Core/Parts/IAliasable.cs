namespace Queries.Core.Parts
{
    /// <summary>
    /// Contract for elements that can be given an alias
    /// </summary>
    /// <typeparam name="T">Type of the element to put an alias on</typeparam>
    public interface IAliasable<out T>
    {
        /// <summary>
        /// Gets the alias of the element
        /// </summary>
        string Alias
        {
            get;
        }

        /// <summary>
        /// Defines the alias of the element
        /// </summary>
        /// <param name="alias"></param>
        /// <returns>The aliased element</returns>
        T As(string alias);
    }
}
