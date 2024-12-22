using FluentAssertions;

using FluentValidation;
using FluentValidation.Results;

using Queries.Core.Parts.Columns;
using Queries.Core.Validators;

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Xunit;

namespace Queries.Core.Tests.Validators;

/// <summary>
/// Unit tests for <see cref="ColumnValidator"/> class.
/// </summary>
public class ColumnValidatorTests : IDisposable
{
    private ColumnValidator _validator;

    public ColumnValidatorTests() => _validator = new ColumnValidator();

    public void Dispose() => _validator = null;

    public static TheoryData<IColumn, Expression<Func<ValidationResult, bool>>, string> ValidateColumnsCases
        => new()
        {
            {
                new FieldColumn("1"),
                vr => vr.IsValid,
                $"new {nameof(FieldColumn)}(1) is a valid usage"
            }
        };


    /// <summary>
    /// Tests various <see cref="IColumn"/>s configurations
    /// </summary>
    /// <param name="column"><see cref="IColumn"/> configuration under tests</param>
    /// <param name="expectation"><see cref="ValidationResult"/>'s expectation.</param>
    /// <param name="because">message that will be displayed if <paramref name="expectation"/> is not met.</param>
    /// <returns></returns>
    [Theory]
    [MemberData(nameof(ValidateColumnsCases))]
    public async Task Validate(IColumn column, Expression<Func<ValidationResult, bool>> expectation, string because) {
        // Act
        ValidationResult vr = await _validator.ValidateAsync(column);

        // Assert
        vr.Should()
            .Match(expectation, because);
    }

    /// <summary>
    /// Tests that <see cref="ColumnValidator"/> can validate instances of <see cref="IValidator{IColumn}"/>.
    /// </summary>
    [Fact]
    public void Should_Validate_IColumn_Instances() => (_validator is IValidator<IColumn>).Should().BeTrue();
}
