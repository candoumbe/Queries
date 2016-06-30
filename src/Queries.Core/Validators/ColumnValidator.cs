﻿using System;
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
                valid = !String.IsNullOrWhiteSpace(tc.Name);
            }
            else if (column is AggregateColumn)
            {
                AggregateColumn ac = (AggregateColumn)column;
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
                if (column is NullColumn)
                {
                    NullColumn nullColumn = column as NullColumn;
                    valid = IsValid(nullColumn.Column) && IsValid(nullColumn.DefaultValue);
                    
                } 
                else if (column is ConcatColumn)
                {
                    ConcatColumn concatColumn = column as ConcatColumn;
                    valid = concatColumn.Columns.All(IsValid);
                } 
                else if (column is LengthColumn)
                {
                    LengthColumn lengthColumn = column as LengthColumn;
                    valid = IsValid(lengthColumn.Column);
                }
            }
           
            return valid;
        }
    }
}