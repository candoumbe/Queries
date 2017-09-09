using System;
using Queries.Core.Parts.Columns;
using System.Linq;
using Queries.Core.Parts.Functions;
using FluentValidation;

namespace Queries.Core.Validators
{
    public class ColumnValidator : AbstractValidator<IColumn>
    {
        public ColumnValidator()
        {
            
        }
    }
}