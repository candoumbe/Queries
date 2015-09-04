using System.Linq;
using Queries.Builders;
using Queries.Parts;
using Queries.Parts.Columns;

namespace Queries.Validators
{
    public class SelectQueryValidator : SelectQueryBaseValidator<SelectQuery>
    {

        public IValidate<IColumn> ColumnValidator { get; private set; }
        public IValidate<Table> TableValidator { get; private set; }


        public SelectQueryValidator()
        {
            ColumnValidator = new ColumnValidator();
            TableValidator = new TableValidator();
        }


        public override bool Validate(SelectQuery query)
        {
            return query != null 
                // SELECT validation
                &&  query.Select != null && query.Select.Any() && query.Select.All(col => ColumnValidator.Validate(col))
                // FROM validation
                && (query.From == null || query.From.All(table => (table is Table && TableValidator.Validate((Table)table)) || (table is SelectTable)));    
        }
    }
}