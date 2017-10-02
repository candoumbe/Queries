namespace Queries.Core.Parts.Clauses
{
    public interface IWhereClause
    {
        /// <summary>
        /// Performs deep cloning of the current instance.
        /// </summary>
        /// <returns></returns>
        IWhereClause Clone();
    }
}