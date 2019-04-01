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

        [Fact]
        public void CtorWithNullArgumentDoesNotThrowArgumentNullException()
        {
            // Arrange
            Action action = () => new Literal(null);

            // Assert
            action.Should().NotThrow<ArgumentException>();
        }

        [Fact]
        public void CtorWithNoArgumentDoesNotThrowArgumentNullException()
        {
            // Arrange
            Action action = () => new Literal();

            // Assert
            action.Should().NotThrow<ArgumentException>();
        }

        [Fact]
        public void CtorThrowsArgumentExceptionWhenArgumentIsNotAPrimitiveType()
        {
            // Arrange
            Action action = () => new Literal(new { prop1 = "prop" });

            // Assert
            ArgumentException exception = action.Should().Throw<ArgumentException>("only bool/int/double/float/long/string are supported").Which;

            exception
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();

            exception.Message.Should().NotBeNullOrWhiteSpace();
        }

        public static IEnumerable<object[]> CtorWithPrimitives
        {
            get
            {
                yield return new object[] { 1, "int value is supported" };
                yield return new object[] { true, "boolean value is supported" };
                yield return new object[] { false, "boolean value is supported" };
                yield return new object[] { 91f, $"{91f.GetType()} is supported" };
                yield return new object[] { 91L, $"{91L.GetType()} is supported" };
                yield return new object[] { new DateTime(1990, 2, 26), $"{nameof(DateTime)} is supported" };
                yield return new object[] { new DateTimeOffset(1990, 2, 26, 14, 30, 0, TimeSpan.Zero), $"{nameof(DateTimeOffset)} is supported" };
            }
        }

        [Theory]
        [MemberData(nameof(CtorWithPrimitives))]
        public void CtorSetInternalValueProperly(object value, string because)
        {
            // Act
            Literal literalColumn = new Literal(value);

            // Assert
            literalColumn.Value.Should().Be(value, because);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("newAlias")]
        public void SettingAlias(string newAlias)
        {
            // Arrange
            Literal column = new Literal("column");

            // Act
            column = column.As(newAlias);

            // Assert
            column.Alias.Should().Be(newAlias);
        }

        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[] { new Literal("firstname"), null, false, "object is null" };
                yield return new object[] { new Literal("firstname"), new Literal("firstname"), true, $"object is a {nameof(Literal)} with exactly the same {nameof(Literal.Value)} and {nameof(Literal.Alias)}" };
                
                {
                    Literal column = new Literal("firstname");
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
                .BeOfType<Literal>().Which.Should()
                .NotBeSameAs(original).And
                .Be(original);
        }
    }
}
