using Queries.Parts;

namespace Queries.Builders.Fluent
{
    public interface IJoinQuery<T>
    {
        IWhereQuery<T> InnerJoin(IClause clause);
    }
}