using Queries.Core.Parts.Clauses;

namespace Queries.Core.Builders.Fluent
{
    /// <summary>
    /// Allows to append a <see cref="IWhereClause"/> to the query being built
    /// </summary>
    /// <typeparam name="T">Type of the query under construction</typeparam>
    public interface IJoinQuery<T> : IInsertable, IBuild<T>
    {
        IWhereQuery<T> Where(IWhereClause clause);
    }
}