using FluentValidation;
using Queries.Core.Builders;

namespace Queries.Core.Validators;

/// <summary>
/// Base class to extend when starting to write 
/// </summary>
/// <typeparam name="T">Type of instances that the validator can handle</typeparam>
public abstract class SelectQueryBaseValidator<T> : AbstractValidator<T> where T : SelectQueryBase
{
}