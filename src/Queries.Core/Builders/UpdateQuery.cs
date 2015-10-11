using System.Collections.Generic;
using Queries.Core.Extensions;
using Queries.Core.Parts;
using Queries.Core.Parts.Clauses;

namespace Queries.Core.Builders
{
    public class UpdateQuery: IQuery
    {
        public Table Table { get; set; }
        public IList<UpdateFieldValue> Values { get; set; }
        public IWhereClause Criteria { get; set; }

        public UpdateQuery(string tableName) : this(tableName?.Table())
        { }
        

        public UpdateQuery(Table table)
        {
            Table = table;
            Values = new List<UpdateFieldValue>();
        }


        public UpdateQuery Set(params UpdateFieldValue[] newValues)
        {
            Values = newValues;
            return this;
        }

        public UpdateQuery Where(IWhereClause clause)
        {
            Criteria = clause;
            return this;
        }
    }
}