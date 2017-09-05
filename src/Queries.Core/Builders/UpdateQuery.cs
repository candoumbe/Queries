using System.Collections.Generic;
using Queries.Core.Parts;
using Queries.Core.Parts.Clauses;
using System;

namespace Queries.Core.Builders
{
    /// <summary>
    /// A query to update a table
    /// </summary>
    public class UpdateQuery: IDataManipulationQuery
    {
        public Table Table { get; }
        public IList<UpdateFieldValue> Values { get; private set; }
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