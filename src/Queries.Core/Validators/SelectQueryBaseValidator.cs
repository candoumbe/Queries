using Queries.Core.Builders;

namespace Queries.Core.Validators
{
    public abstract class SelectQueryBaseValidator<T> : IValidate<T> where T : SelectQueryBase
    {
        public abstract bool IsValid(T element);
    }
}