using System.Collections.Generic;
using Queries.Core.Parts;
using Queries.Core.Parts.Clauses;

namespace Queries.Core.Builders
{
    public class UpdateQuery: IQuery
    {
        public Table Table { get; set; }
        public IList<UpdateFieldValue> Set { get; set; }
        public IWhereClause Where { get; set; }
    }
}