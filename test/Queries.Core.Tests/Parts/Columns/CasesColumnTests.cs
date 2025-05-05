using FluentAssertions;
using Queries.Core.Parts.Columns;
using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Parts.Columns;

[UnitTest]
public class CasesColumnTests(ITestOutputHelper outputHelper)
{
    [Fact]
    public void GivenNullParameter_Ctor_ThrowsArgumentNullException()
    {
        // Arrange
        Action action = () => _ = new CasesColumn(cases: null);

        // Act & Assert
        action.Should()
            .ThrowExactly<ArgumentNullException>()
            .Which.ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    public static TheoryData<CasesColumn, object, bool, string> EqualsCases = new()
    {
        {
            new CasesColumn([
                When("Age".Field().GreaterThan(18), then : true),
                When("Age".Field().IsNull(), then : false)
            ]),
            null,
            false,
            "object is null"
        },
        {
            new CasesColumn([
                When("Age".Field().GreaterThan(18), then : true),
                When("Age".Field().IsNull(), then : false)
            ]),
            new CasesColumn([
                When("Age".Field().GreaterThan(18), then : true),
                When("Age".Field().IsNull(), then : false)
            ]),
            true,
            $"object is a {nameof(CasesColumn)} with exactly the same {nameof(CasesColumn.Cases)} and {nameof(CasesColumn.Alias)}"
        },
        {
            new CasesColumn([
                When("Age".Field().GreaterThan(18), then : true),
                When("Age".Field().IsNull(), then : false)
            ]),
            Cases(
                When("Age".Field().GreaterThan(18), then : true),
                When("Age".Field().IsNull(), then : false)
            ),
            true,
            $"comparing a {nameof(CasesColumn)} and an instance built with fluent {nameof(Cases)} method"
        },
        {
            Cases(
                When("Age".Field().GreaterThan(18), then : true),
                When("Age".Field().IsNull(), then : false)
            ),
            new SelectColumn(Select(1.Literal())),
            false,
            $"{nameof(CasesColumn)} is always != exactly the same {nameof(SelectColumn)}"
        },
        {
            Cases(
                When("Age".Field().GreaterThan(18), then: true),
                When("Age".Field().IsNull(), then: false)
            ),
            Cases(
                When("Age".Field().GreaterThan(18), then: true),
                When("Age".Field().IsNull(), then: false)
            ),
            true,
            "Equals with same instance"
        }
    };

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualTests(CasesColumn first, object second, bool expectedResult, string reason)
    {
        outputHelper.WriteLine($"First : {first}");
        outputHelper.WriteLine($"Second : {second}");

        // Act
        bool actualResult = first.Equals(second);

        // Assert
        actualResult.Should().Be(expectedResult, reason);
    }
}