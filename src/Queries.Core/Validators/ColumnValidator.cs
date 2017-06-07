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
            else if (column is FieldColumn tc)
            {
                valid = !string.IsNullOrWhiteSpace(tc.Name);
            }
            else if (column is AggregateFunction ac)
            {
                valid = IsValid(ac.Column);
            }
            else if (column is SelectColumn inlineSelectQuery)
            {
                SelectQueryValidator validator = new SelectQueryValidator();
                valid = validator.IsValid(inlineSelectQuery.SelectQuery);
            }
            else if (column is IFunctionColumn)
            {
                if (column is NullFunction nullColumn)
                {
                    valid = IsValid(nullColumn.Column) && IsValid(nullColumn.DefaultValue);

                }
                else if (column is ConcatFunction concatColumn)
                {
                    valid = concatColumn.Columns.All(IsValid);
                }
                else if (column is LengthFunction lengthColumn)
                {
                    valid = IsValid(lengthColumn.Column);
                }
            }

            return valid;
        }
    }
}