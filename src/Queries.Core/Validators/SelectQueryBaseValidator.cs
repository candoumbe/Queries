using FluentValidation;
using Queries.Core.Builders;

namespace Queries.Core.Validators
{
    public abstract class SelectQueryBaseValidator<T> : AbstractValidator<T> where T : SelectQueryBase
    {
    }
}