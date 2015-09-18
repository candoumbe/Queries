using Queries.Builders;

namespace Queries.Validators
{
    public class DeleteQueryValidator : IValidate<DeleteQuery>
    {
        public bool IsValid(DeleteQuery element)
        {
            return element != null && new TableValidator().IsValid(element.Table);
        }
    }
}