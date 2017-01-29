using Queries.Core.Parts.Columns;

namespace Queries.Core.Validators
{
    public class InlineSelectQueryColumnValidator : IValidate<SelectColumn>
    {

        public bool IsValid(SelectColumn query) => false;
    }
}