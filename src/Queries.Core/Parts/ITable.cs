namespace Queries.Core.Parts
{
    /// <summary>
    /// Marker interface for elements that can be used where a table 
    /// </summary>
    public interface ITable
    {
        /// <summary>
        /// Performs a deep copy of the  current instance
        /// </summary>
        /// <returns></returns>
        ITable Clone();
    }
}