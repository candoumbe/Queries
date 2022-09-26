using System;
using System.Collections.Generic;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;
using FluentAssertions;
using Queries.Core.Attributes;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Functions;

[UnitTest]
[Feature(nameof(MaxFunction))]
[Feature("Functions")]
public class MaxFunctionTests
{
    [Fact]
    public void ConstructorTestWithNullStringArgument()
    {
        Action action = () => new MaxFunction((string)null);

        action.Should().ThrowExactly<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ConstructorTestWithEmptyStringArgument()
    {
        Action action = () => new MaxFunction(string.Empty);

        action.Should().ThrowExactly<ArgumentOutOfRangeException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ConstructorTestWithWhitespaceStringArgument()
    {
        Action action = () => new MaxFunction("   ");

        action.Should().ThrowExactly<ArgumentOutOfRangeException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ConstructorTestWithNullColumnArgument()
    {
        Action action = () => new MaxFunction((IColumn)null);

        action.Should().ThrowExactly<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ConstructorTestColumnArgument() => new MaxFunction("age").Type.Should().Be(AggregateType.Max);

    public static IEnumerable<object[]> AsTestCases
    {
        get
        {
            yield return new object[]
            {
                new MaxFunction("age".Field()),
                null,
            };

            yield return new object[]
            {
                new MaxFunction("age".Field()).As(string.Empty),
                string.Empty,
            };
        }
    }

    [Theory]
    [MemberData(nameof(AsTestCases))]
    public void SettingAliasTest(MaxFunction column, string expectedAlias)
        => column.Alias.Should().Be(expectedAlias);

    [Fact]
    public void HasFunctionAttribute() => typeof(MaxFunction).Should()
            .BeDecoratedWithOrInherit<FunctionAttribute>($"{nameof(MaxFunction)} must be marked with {nameof(FunctionAttribute)}");

    [Fact]
    public void IsAggregateFunction() => typeof(MaxFunction).Should()
        .BeAssignableTo<AggregateFunction>();
}