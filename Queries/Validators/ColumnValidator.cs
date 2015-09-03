using System;
using Queries.Parts;
using Queries.Parts.Columns;

namespace Queries.Validators
{
    public class ColumnValidator : IValidate<IColumn>
    {
        

        public bool Validate(IColumn column)
        {

            bool valid = false;


            if (column is LiteralColumn)
            {
                valid = true;
            } 
            else if (column is TableColumn)
            {
                TableColumn tc = (TableColumn) column;
                valid = !String.IsNullOrWhiteSpace(tc.Name);
            }
            else if (column is AggregateColumn)
            {
                AggregateColumn ac = (AggregateColumn)column;
                valid = Validate(ac.Column);
            } 
            else if (column is SelectColumn)
            {
                SelectColumn inlineSelectQuery = (SelectColumn) column;
                SelectQueryValidator validator = new SelectQueryValidator();
                valid = validator.Validate(inlineSelectQuery.SelectQuery);


            }
           


            return valid;
        }
    }


    public class TableValidator : IValidate<TableTerm>
    {
        public bool Validate(TableTerm table)
        {
            return table != null && !String.IsNullOrWhiteSpace(table.Name);
        }
    }
}