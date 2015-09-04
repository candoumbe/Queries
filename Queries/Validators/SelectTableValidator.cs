using Queries.Parts;

namespace Queries.Validators
{
    public class SelectTableValidator : IValidate<SelectTable>
    {
        public bool Validate(SelectTable table)
        {
            return table != null;
        }
    }
}