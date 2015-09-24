using Queries.Core.Parts.Clauses;

namespace Queries.Core.Parts.Joins
{
    public interface IJoin
    {
        JoinType JoinType { get; }
        Table Table { get; }
        IWhereClause On { get; }
    }
}
