using FluentAssertions;
using Queries.Core.Attributes;
using Queries.Core.Parts.Functions;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Columns
{
    [UnitTest]
    [Feature(nameof(CountFunction))]
    [Feature("Functions")]
    public class CountFunctionTests
    {
        [Fact]
        public void ConstructorTestWithNullArgument()
        {
            // Act
            Action action = () => new CountFunction(null);

            // Assert
            action.Should().Throw<ArgumentNullException>($"{nameof(CountFunction)} constructor called with null argument").Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        public static IEnumerable<object[]> AsTestCases
        {
            get
            {
                yield return new object[]
                {
                    new CountFunction("firstname".Field()),
                    null,
                };

                yield return new object[]
                {
                    new CountFunction("firstname".Field()).As(string.Empty),
                    string.Empty,
                };
            }
        }

        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(CountFunction column, string expectedAlias)
            => column.Alias.Should().Be(expectedAlias);

        [Fact]
        public void HasFunctionAttribute() => typeof(CountFunction).Should()
                .BeDecoratedWithOrInherit<FunctionAttribute>($"{nameof(CountFunction)} must be marked with {nameof(FunctionAttribute)}");
    }
}