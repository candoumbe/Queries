using Queries.Builders;
using Queries.Extensions;

namespace Queries.Validators
{
    public class CreateViewQueryValidator : IValidate<CreateViewQuery>
    {
        public bool IsValid(CreateViewQuery query)
        {
            return query != null 
                && new TableValidator().IsValid(query.Name.Table())
                && new SelectQueryValidator().IsValid(query.Select);
        }
    }
}
