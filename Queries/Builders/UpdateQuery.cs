using System.Collections.Generic;
using Queries.Parts;

namespace Queries.Builders
{
    public class UpdateQuery
    {
        public TableTerm Table { get; set; }
        public IList<UpdateFieldValue> Set { get; set; }
        public IClause Where { get; set; }
    }
}