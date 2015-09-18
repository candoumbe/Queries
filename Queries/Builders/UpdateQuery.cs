using System.Collections.Generic;
using Queries.Parts;
using Queries.Parts.Clauses;

namespace Queries.Builders
{
    public class UpdateQuery
    {
        public Table Table { get; set; }
        public IList<UpdateFieldValue> Set { get; set; }
        public IWhereClause Where { get; set; }
    }
}