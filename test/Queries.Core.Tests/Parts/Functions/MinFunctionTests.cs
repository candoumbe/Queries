using System;
using System.Collections.Generic;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;
using FluentAssertions;
using Queries.Core.Attributes;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Columns
{
    [UnitTest]
    [Feature(nameof(MinFunction))]
    [Feature("Functions")]
    public class MinFunctionTests
    {
        [Fact]
        public void ConstructorTestWithNullStringArgument()
        {
            Action action = () => new MinFunction((string)null);

            action.Should().ThrowExactly<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ConstructorTestWithEmptyStringArgument()
        {
            Action action = () => new MinFunction(string.Empty);

            action.Should().ThrowExactly<ArgumentOutOfRangeException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void ConstructorTestWithWhitespaceStringArgument(string columnName)
        {
            Action action = () => new MinFunction(columnName);

            action.Should().ThrowExactly<ArgumentOutOfRangeException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ConstructorTestWithNullColumnArgument()
        {
            Action action = () => new MinFunction((IColumn) null);

            action.Should().ThrowExactly<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ConstructorTestColumnArgument() => new MinFunction("age").Type.Should().Be(AggregateType.Min);

        public static IEnumerable<object[]> AsTestCases
        {
            get
            {
                yield return new object[]
                {
                    new MinFunction("age".Field()),
                    null,
                };

                yield return new object[]
                {
                    new MinFunction("age".Field()).As(string.Empty),
                    string.Empty,
                };
            }
        }

        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(MinFunction column, string expectedAlias)
            => column.Alias.Should().Be(expectedAlias);

        [Fact]
        public void HasFunctionAttribute() => typeof(MinFunction).Should()
            .BeDecoratedWithOrInherit<FunctionAttribute>($"{nameof(MinFunction)} must be marked with {nameof(FunctionAttribute)}");
    }
}