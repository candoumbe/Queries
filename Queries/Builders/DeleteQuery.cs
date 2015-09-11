using Queries.Parts;
using Queries.Parts.Clauses;

namespace Queries.Builders
{
    public class DeleteQuery
    {
        public Table Table { get; set; }

        public IWhereClause Where { get; set; }
    }
}