namespace Queries.Parts
{
    public interface IAliasable<T>
    {
        /// <summary>
        /// Gets the alias
        /// </summary>
        string Alias
        {
            get;
        }

        /// <summary>
        /// Defines the alias of the element
        /// </summary>
        /// <param name="alias"></param>
        T As(string alias);
    }
}
