namespace Queries.Core.Parts
{
    public interface ITable
    {
        /// <summary>
        /// Performs a deep copy of the  current instance
        /// </summary>
        /// <returns></returns>
        ITable Clone();
    }
}