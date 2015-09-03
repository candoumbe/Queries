using Queries.Parts;

namespace Queries.Builders.Fluent
{
    public interface ISqlFrom
    {
        ISqlWhere From(TableTerm table, params TableTerm[] tables);

        ISqlWhere From(SelectQuery select);

        ISqlWhere Where(IClause clause);
    }
}