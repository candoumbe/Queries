namespace Queries.Core.Parts.Clauses
{
    public interface IHavingClause
    {
        /// <summary>
        /// Performs a deep copy of the current instance.
        /// </summary>
        /// <returns></returns>
        IHavingClause Clone();
    }
}