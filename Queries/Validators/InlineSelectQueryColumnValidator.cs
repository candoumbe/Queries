using Queries.Parts.Columns;

namespace Queries.Validators
{
    public class InlineSelectQueryColumnValidator : IValidate<SelectColumn>
    {
        public bool Validate(SelectColumn query)
        {

            return false;
        }
    }
}