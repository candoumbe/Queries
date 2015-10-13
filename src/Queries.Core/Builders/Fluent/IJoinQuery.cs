using Queries.Core.Parts.Clauses;

namespace Queries.Core.Builders.Fluent
{
    public interface IJoinQuery<T> : IInsertable
    {
        IWhereQuery<T> Where(IWhereClause clause);
    }
}