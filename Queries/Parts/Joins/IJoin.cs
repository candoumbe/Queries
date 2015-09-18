using Queries.Parts.Clauses;

namespace Queries.Parts.Joins
{
    public interface IJoin
    {
        JoinType JoinType { get; }
        Table Table { get; }
        IWhereClause On { get; }
    }
}
