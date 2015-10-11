using Queries.Core.Parts.Clauses;

namespace Queries.Core.Builders.Fluent
{
    public interface IJoinQuery<T>
    {
        IWhereQuery<T> Where(IWhereClause clause);
    }
}