using Queries.Parts;

namespace Queries.Validators
{
    public class SelectTableValidator : IValidate<SelectTable>
    {
        public bool IsValid(SelectTable table)
        {
            return table != null;
        }
    }
}