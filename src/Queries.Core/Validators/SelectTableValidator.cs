using Queries.Core.Parts;

namespace Queries.Core.Validators
{
    public class SelectTableValidator : IValidate<SelectTable>
    {
        public bool IsValid(SelectTable table) => table != null;
    }
}