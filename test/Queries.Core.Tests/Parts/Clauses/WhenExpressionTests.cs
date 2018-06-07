using FluentAssertions;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Parts.Clauses
{
    public class WhenExpressionTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public WhenExpressionTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;


        [Fact]
        public void GivenNullParameter_Ctor_ThrowsArgumentNullException()
        {
            // Arrange
            Action action = () => new WhenExpression(criterion : null, then : 18);

            // Act & Assert
            action.ShouldThrow<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[] { When("Age".Field().GreaterThan(18), then : true), null, false, "object is null" };
                yield return new object[] {
                    When("Age".Field().GreaterThan(18), then : true),
                    When("Age".Field().GreaterThan(18), then : true),
                    true,
                    $"object is a {nameof(WhenExpression)} with exactly the same {nameof(WhenExpression.Criterion)} and {nameof(WhenExpression.ThenValue)}" };
                yield return new object[] { 
                    When("Age".Field().GreaterThan(18), then : true),
                    When("Age".Field().GreaterThan(18), then : false),
                    false,
                    $"object is a {nameof(WhenExpression)} with exactly the same {nameof(WhenExpression.Criterion)} but different {nameof(WhenExpression.ThenValue)}"
                };

                yield return new object[] {
                    When("Age".Field().GreaterThan(18), then : true),
                    When("Age".Field().GreaterThan(21), then : true),
                    false,
                    $"object is a {nameof(WhenExpression)} with exactly different {nameof(WhenExpression.Criterion)} but same {nameof(WhenExpression.ThenValue)}"
                };

                {
                    WhenExpression whenExpression = When("Age".Field().GreaterThan(18), then: true);
                    yield return new object[] { whenExpression, whenExpression, true, "Equals with same instance" };
                }

            }
        }

        [Theory]
        [MemberData(nameof(EqualsCases))]
        public void EqualTests(WhenExpression first, object second, bool expectedResult, string reason)
        {
            _outputHelper.WriteLine($"First : {first}");
            _outputHelper.WriteLine($"Second : {second}");

            // Act
            bool actualResult = first.Equals(second);

            // Assert
            actualResult.Should().Be(expectedResult, reason);
        }
    }
}
