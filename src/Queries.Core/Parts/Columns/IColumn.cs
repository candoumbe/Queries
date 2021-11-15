namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// Classes that implement this interface are 
    /// </summary>
    public interface IColumn
    {
        /// <summary>
        /// Performs a deep copy of the current instance
        /// </summary>
        /// <returns></returns>
        IColumn Clone();
    }
}