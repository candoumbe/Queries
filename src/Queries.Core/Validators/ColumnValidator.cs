using System;
using Queries.Core.Parts.Columns;
using System.Linq;
using Queries.Core.Parts.Functions;

namespace Queries.Core.Validators
{
    public class ColumnValidator : IValidate<IColumn>
    {
        public bool IsValid(IColumn column)
        {

            bool valid = false;


            if (column is LiteralColumn)
            {
                valid = true;
            } 
            else if (column is FieldColumn)
            {
                FieldColumn tc = (FieldColumn) column;
                valid = !string.IsNullOrWhiteSpace(tc.Name);
            }
            else if (column is AggregateFunction)
            {
                AggregateFunction ac = (AggregateFunction)column;
                valid = IsValid(ac.Column);
            } 
            else if (column is SelectColumn)
            {
                SelectColumn inlineSelectQuery = (SelectColumn) column;
                SelectQueryValidator validator = new SelectQueryValidator();
                valid = validator.IsValid(inlineSelectQuery.SelectQuery);
            }
            else if (column is IFunctionColumn)
            {
                if (column is NullFunction)
                {
                    NullFunction nullColumn = column as NullFunction;
                    valid = IsValid(nullColumn.Column) && IsValid(nullColumn.DefaultValue);
                    
                } 
                else if (column is ConcatFunction)
                {
                    ConcatFunction concatColumn = column as ConcatFunction;
                    valid = concatColumn.Columns.All(IsValid);
                } 
                else if (column is LengthFunction)
                {
                    LengthFunction lengthColumn = column as LengthFunction;
                    valid = IsValid(lengthColumn.Column);
                }
            }
           
            return valid;
        }
    }
}