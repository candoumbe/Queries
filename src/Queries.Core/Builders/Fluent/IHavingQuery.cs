using Queries.Core.Parts.Clauses;

namespace Queries.Core.Builders.Fluent
{
    public interface IHavingQuery<T> : IUnionQuery<T>
    {
        IOrderQuery<T> Having(IHavingClause clause);
    }
}