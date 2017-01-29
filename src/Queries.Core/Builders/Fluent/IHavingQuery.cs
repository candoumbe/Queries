using Queries.Core.Parts.Clauses;

namespace Queries.Core.Builders.Fluent
{
    public interface IHavingQuery<T> : IUnionQuery<T>
    {
        ISortQuery<T> Having(IHavingClause clause);
    }
}