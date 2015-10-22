using System.Collections.Generic;
using System.Linq;
using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Parts;
using Queries.Core.Parts.Columns;
using Queries.Core.Validators;

namespace Queries.Renderers.Neo4J.Validators
{

    public class Neo4JInsertIntoQueryValidator : IValidate<InsertIntoQuery>
    {
        public bool IsValid(InsertIntoQuery element)
        {
            bool valid = false;

            if (element != null)
            {
                
            }

            return valid;
        }
    }

    public class Neo4JSelectQueryValidator : IValidate<SelectQuery>
    {
        public bool IsValid(SelectQuery selectQuery)
        {
            bool valid = false;

            if (selectQuery != null)
            {
                IEnumerable<ITable> tables = selectQuery.Tables;
                IEnumerable<IColumn> columns = selectQuery.Columns;
                if (columns.Any()
                    && columns.All(col => !string.IsNullOrWhiteSpace((col as FieldColumn)?.Name) // columns must all have the name set
                        && !"*".Equals((col as FieldColumn).Name)  // the '*' is not allowed as column name
                        && !"*".Equals((col as FieldColumn).Alias))
                    && tables.Any()) // the '*' is not allowed as column alias
                {
                    valid = true;
                }
            }
                
            

            return valid;
        }
    }
}
