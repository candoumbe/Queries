using System;
using FluentAssertions;
using Queries.Core.Attributes;
using Queries.Core.Parts.Functions;
using Xunit;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Functions;

[UnitTest]
[Feature(nameof(CountFunction))]
[Feature("Functions")]
public class CountFunctionTests
{
    [Fact]
    public void ConstructorTestWithNullArgument()
    {
        // Act
        Action action = () => _ = new CountFunction(null);

        // Assert
        action.Should().Throw<ArgumentNullException>($"{nameof(CountFunction)} constructor called with null argument").Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    public static TheoryData<CountFunction, string> AsTestCases
        => new()
        {
            {
                new CountFunction("firstname".Field()),
                null
            },
            {
                new CountFunction("firstname".Field()).As(string.Empty),
                string.Empty
            }
        };

    [Theory]
    [MemberData(nameof(AsTestCases))]
    public void SettingAliasTest(CountFunction column, string expectedAlias)
        => column.Alias.Should().Be(expectedAlias);

    [Fact]
    public void HasFunctionAttribute() => typeof(CountFunction).Should()
            .BeDecoratedWithOrInherit<FunctionAttribute>($"{nameof(CountFunction)} must be marked with {nameof(FunctionAttribute)}");
}