using Queries.Parts.Clauses;

namespace Queries.Builders.Fluent
{
    public interface IJoinQuery<T>
    {
        IWhereQuery<T> InnerJoin(IWhereClause clause);
    }
}