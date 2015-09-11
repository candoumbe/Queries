using Queries.Parts.Columns;

namespace Queries.Validators
{
    public class InlineSelectQueryColumnValidator : IValidate<SelectColumn>
    {
        public bool IsValid(SelectColumn query)
        {

            return false;
        }
    }
}