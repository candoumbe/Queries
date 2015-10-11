using Queries.Core.Builders;

namespace Queries.Core.Validators
{
    public class DeleteQueryValidator : IValidate<DeleteQuery>
    {
        public bool IsValid(DeleteQuery element) => !string.IsNullOrWhiteSpace(element?.Table);
    }
}