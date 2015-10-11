using Queries.Core.Builders;
using Queries.Core.Extensions;

namespace Queries.Core.Validators
{
    public class TruncateQueryValidator : IValidate<TruncateQuery>
    {
        public bool IsValid(TruncateQuery query)
        {
            return query != null && new TableValidator().IsValid(query.Name.Table());
        }
    }
}