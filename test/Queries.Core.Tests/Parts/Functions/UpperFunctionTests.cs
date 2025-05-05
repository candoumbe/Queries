using System;
using System.Collections.Generic;
using FluentAssertions;
using Queries.Core.Attributes;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;
using Xunit;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Functions;

[UnitTest]
[Feature(nameof(UpperFunction))]
[Feature("Functions")]
public class UpperFunctionTests
{
    [Fact]
    public void ConstructorTestWithNullStringArgument()
    {
        Action action = () => _ = new UpperFunction((string)null);

        action.Should().ThrowExactly<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ConstructorTestWithNullColumnArgument()
    {
        Action action = () => _ = new UpperFunction((IColumn) null);

        action.Should().ThrowExactly<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(nameof(AsTestCases))]
    public void SettingAliasTest(UpperFunction column, string expectedAlias)
        => column.Alias.Should().Be(expectedAlias);

    public static TheoryData<UpperFunction, string> AsTestCases
        => new()
        {
            { new UpperFunction("firstname".Field()), null },
            { new UpperFunction("firstname".Field()).As(string.Empty), string.Empty }
        };

    [Fact]
    public void HasFunctionAttribute()
        => typeof(UpperFunction).Should()
            .BeDecoratedWith<FunctionAttribute>($"{nameof(UpperFunction)} must be marked with {nameof(FunctionAttribute)}");
}