namespace Queries.Core.Parts.Clauses
{
    public interface IWhereClause
    {
        /// <summary>
        /// Indicates if the current instance has parameters.
        /// </summary>
        bool IsParameterized { get; }

        /// <summary>
        /// Performs deep cloning of the current instance.
        /// </summary>
        /// <returns></returns>
        IWhereClause Clone();
    }
}