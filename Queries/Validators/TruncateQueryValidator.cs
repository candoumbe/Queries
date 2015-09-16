using Queries.Builders;
using Queries.Extensions;

namespace Queries.Validators
{
    public class TruncateQueryValidator : IValidate<TruncateQuery>
    {
        public bool IsValid(TruncateQuery query)
        {
            return query != null && new TableValidator().IsValid(query.Name.Table());
        }
    }
}