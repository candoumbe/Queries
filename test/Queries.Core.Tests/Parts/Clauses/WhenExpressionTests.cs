using AwesomeAssertions;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Parts.Clauses;

public class WhenExpressionTests(ITestOutputHelper outputHelper)
{
    [Fact]
    public void GivenNullParameter_Ctor_ThrowsArgumentNullException()
    {
        // Arrange
        Action action = () =>_ = new WhenExpression(criterion: null, then: 18);

        // Act & Assert
        action.Should()
            .ThrowExactly<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    public static TheoryData<WhenExpression, object, bool, string> EqualsCases => new()
    {
        { When("Age".Field().GreaterThan(18), then: true), null, false, "object is null" },
        {
            When("Age".Field().GreaterThan(18), then : true),
            When("Age".Field().GreaterThan(18), then : true),
            true,
            $"object is a {nameof(WhenExpression)} with exactly the same {nameof(WhenExpression.Criterion)} and {nameof(WhenExpression.ThenValue)}"
        },
        {
            When("Age".Field().GreaterThan(18), then : true),
            When("Age".Field().GreaterThan(18), then : false),
            false,
            $"object is a {nameof(WhenExpression)} with exactly the same {nameof(WhenExpression.Criterion)} but different {nameof(WhenExpression.ThenValue)}"
        },
        {
            When("Age".Field().GreaterThan(18), then : true),
            When("Age".Field().GreaterThan(21), then : true),
            false,
            $"object is a {nameof(WhenExpression)} with exactly different {nameof(WhenExpression.Criterion)} but same {nameof(WhenExpression.ThenValue)}"
        },
        {
            When("Age".Field().GreaterThan(18), then: true),
            When("Age".Field().GreaterThan(18), then: true),
            true,
            "Equals with same instance"
        }
    };

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualTests(WhenExpression first, object second, bool expectedResult, string reason)
    {
        outputHelper.WriteLine($"First : {first}");
        outputHelper.WriteLine($"Second : {second}");

        // Act
        bool actualResult = first.Equals(second);

        // Assert
        actualResult.Should().Be(expectedResult, reason);
    }
}