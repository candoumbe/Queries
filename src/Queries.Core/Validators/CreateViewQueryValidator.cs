using Queries.Core.Builders;
using Queries.Core.Extensions;

namespace Queries.Core.Validators
{
    public class CreateViewQueryValidator : IValidate<CreateViewQuery>
    {
        public bool IsValid(CreateViewQuery query) => query != null
    && new TableValidator().IsValid(query.ViewName.Table())
    && new SelectQueryValidator().IsValid(query.SelectQuery);
    }
}
