using Queries.Core.Builders;
using Queries.Core.Parts;
using Queries.Core.Parts.Columns;
using System.Linq;

namespace Queries.Core.Validators
{
    public class SelectQueryValidator : SelectQueryBaseValidator<SelectQuery>
    {

        public IValidate<IColumn> ColumnValidator { get;}
        public IValidate<Table> TableValidator { get; }


        public SelectQueryValidator()
        {
            ColumnValidator = new ColumnValidator();
            TableValidator = new TableValidator();
        }


        public override bool IsValid(SelectQuery query) => 
            query != null && (query.Columns?.Any() ?? false)
            && query.Columns.All(col => ColumnValidator.IsValid(col)) 
                && (query.Tables?.All(t => (t is Table table && TableValidator.IsValid((Table)t)) || (t is SelectTable)) ?? false);
    }
}