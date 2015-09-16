using Queries.Parts.Clauses;

namespace Queries.Builders.Fluent
{
    public interface IJoinQuery<T>
    {
        IWhereQuery<T> Where(IWhereClause clause);
    }
}