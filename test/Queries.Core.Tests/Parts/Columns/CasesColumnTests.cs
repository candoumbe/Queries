using FluentAssertions;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Parts.Columns
{
    public class CasesColumnTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public CasesColumnTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        [Fact]
        public void GivenNullParameter_Ctor_ThrowsArgumentNullException()
        {
            // Arrange
            Action action = () => new CasesColumn(cases: null);

            // Act & Assert
            action.Should()
                .ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[] {
                    new CasesColumn(new[] {
                        When("Age".Field().GreaterThan(18), then : true),
                        When("Age".Field().IsNull(), then : false)
                    }),
                    null,
                    false,
                    "object is null" };
                yield return new object[] {  new CasesColumn(new[] {
                        When("Age".Field().GreaterThan(18), then : true),
                        When("Age".Field().IsNull(), then : false)
                    }),
                    new CasesColumn(new[] {
                        When("Age".Field().GreaterThan(18), then : true),
                        When("Age".Field().IsNull(), then : false)
                    }), true, $"object is a {nameof(CasesColumn)} with exactly the same {nameof(CasesColumn.Cases)} and {nameof(CasesColumn.Alias)}" };

                yield return new object[] {  new CasesColumn(new[] {
                        When("Age".Field().GreaterThan(18), then : true),
                        When("Age".Field().IsNull(), then : false)
                    }),
                    Cases(
                        When("Age".Field().GreaterThan(18), then : true),
                        When("Age".Field().IsNull(), then : false)
                    ), true, $"comparing a {nameof(CasesColumn)} and an instance built with fluent {nameof(Cases)} method" };

                yield return new object[] {  Cases(
                        When("Age".Field().GreaterThan(18), then : true),
                        When("Age".Field().IsNull(), then : false)
                    ), new SelectColumn(Select(1.Literal())), false, $"{nameof(CasesColumn)} is always != exactly the same {nameof(SelectColumn)}" };

                {
                    CasesColumn column = Cases(
                        When("Age".Field().GreaterThan(18), then: true),
                        When("Age".Field().IsNull(), then: false)
                    );
                    yield return new object[] { column, column, true, "Equals with same instance" };
                }
            }
        }

        [Theory]
        [MemberData(nameof(EqualsCases))]
        public void EqualTests(CasesColumn first, object second, bool expectedResult, string reason)
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
