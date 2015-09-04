using Queries.Builders;

namespace Queries.Validators
{
    public abstract class SelectQueryBaseValidator<T> : IValidate<T> where T : SelectQueryBase
    {
        public abstract bool Validate(T element);
    }
}