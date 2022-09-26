using Queries.Core.Builders;

namespace Queries.Core.Validators;

/// <summary>
/// Validates <see cref="SelectQuery"/> instance
/// </summary>
public class SelectQueryValidator : SelectQueryBaseValidator<SelectQuery>
{
    /// <summary>
    /// Builds a new <see cref="SelectQueryValidator"/> instance.
    /// </summary>
    public SelectQueryValidator()
    {

    }
}