using FluentAssertions;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Columns
{
    [UnitTest]
    [Feature(nameof(Literal))]
    [Feature("Column")]
    public class LiteralTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public LiteralTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public void Dispose() => _outputHelper = null;

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("newAlias")]
        public void SettingAlias(string newAlias)
        {
            // Arrange
            Literal column = "column".Literal().As(newAlias);

            // Act
            column = column.As(newAlias);

            // Assert
            column.Alias.Should().Be(newAlias);
        }

        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[] { "firstname".Literal(), null, false, "object is null" };
                yield return new object[] { "firstname".Literal(), "firstname".Literal(), true, $"object is a {nameof(Literal)} with exactly the same {nameof(Literal.Value)} and {nameof(Literal.Alias)}" };
                
                {
                    Literal column =  "firstname".Literal();
                    yield return new object[] { column, column, true, "Equals with same instance" };
                }
            }
        }

        [Theory]
        [MemberData(nameof(EqualsCases))]
        public void EqualTests(Literal first, object second, bool expectedResult, string reason)
        {
            _outputHelper.WriteLine($"{nameof(first)} : {first}");
            _outputHelper.WriteLine($"{nameof(second)} : {second}");

            // Act
            bool actualResult = first.Equals(second);

            // Assert
            actualResult.Should().Be(expectedResult, reason);
        }

        public static IEnumerable<object[]> CloneCases
        {
            get
            {
                yield return new[] { 1.Literal() };
                yield return new[] { "Bruce".Literal() };
            }
        }

        [Theory]
        [MemberData(nameof(CloneCases))]
        public void CloneTest(Literal original)
        {
            // Act
            IColumn copie = original.Clone();

            // Assert
            copie.Should()
                .BeAssignableTo<Literal>().Which.Should()
                .NotBeSameAs(original).And
                .Be(original);
        }
    }
}
