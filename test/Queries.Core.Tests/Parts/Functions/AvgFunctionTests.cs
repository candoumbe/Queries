using AwesomeAssertions;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;
using System;
using FsCheck.Xunit;
using Xunit;
using Xunit.Abstractions;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using Queries.Core.Attributes;
using TestsHelpers;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Functions;

[UnitTest]
[Feature(nameof(AvgFunction))]
[Feature("Functions")]
public class AvgFunctionTests(ITestOutputHelper outputHelper)
{
    [Fact]
    public void CtorThrowsArgumentNullExceptionIfColumnParameterIsNull()
    {
        // Act
        Action action = () => _ = new AvgFunction((IColumn) null);

        // Assert
        action.Should().Throw<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void HasFunctionAttribute() => typeof(AvgFunction).Should()
        .BeDecoratedWithOrInherit<FunctionAttribute>($"{nameof(AvgFunction)} must be marked with {nameof(FunctionAttribute)}");

    public static TheoryData<AvgFunction, object, bool, string> EqualsCases => new()
    {
        { Avg("Age".Field()), null, false, $"comparing {nameof(AvgFunction)} with a null instance" },
        { Avg("Age".Field()), Avg("Age".Field()), true, $"comparing two {nameof(AvgFunction)} instances with same column names" },
        { Avg("Age".Field()), Avg("Age".Field()), true, $"comparing {nameof(AvgFunction)} instance to itself" }
    };

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualTests(AvgFunction first, object second, bool expectedResult, string reason)
    {
        outputHelper.WriteLine($"{nameof(first)} : {first}");
        outputHelper.WriteLine($"{nameof(second)} : {second}");

        // Act
        bool actualResult = first.Equals(second);

        // Assert
        actualResult.Should().Be(expectedResult, reason);
    }

    [Fact]
    public void ConstructorTestWithNullStringArgument()
    {
        Action action = () => _ = new AvgFunction((string)null);

        action.Should()
            .ThrowExactly<ArgumentNullException>().Which
            .ParamName.Should()
                .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ConstructorTestWithEmptyStringArgument()
    {
        Action action = () => _ = new AvgFunction(string.Empty);

        action.Should()
            .ThrowExactly<ArgumentOutOfRangeException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ConstructorTestWithWhitespaceStringArgument()
    {
        Action action = () => _ = new AvgFunction("   ");

        action.Should()
            .ThrowExactly<ArgumentOutOfRangeException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ConstructorTestWithNullColumnArgument()
    {
        Action action = () => _ = new AvgFunction((IColumn)null);

        action.Should()
            .ThrowExactly<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ConstructorTestColumnArgument() => new AvgFunction("age").Type
        .Should().Be(AggregateType.Average);

    [Property(Arbitrary = [typeof(QueryGenerators)])]
    public void SettingAliasTest(AvgFunction column, string newAlias)
    {
        // Act
        AggregateFunction actual = column.As(newAlias);

        // Assert
        _ = newAlias switch
        {
            null => actual.Alias.Should().BeEmpty(),
            string value when string.IsNullOrEmpty(value) => column.Alias.Should().BeEmpty(),
            _ => column.Alias.Should().Be(newAlias)
        };
    }

    public static TheoryData<AvgFunction> CloneCases => new()
    {
        { new AvgFunction("Firstname") },
        { new AvgFunction("Firstname".Field()) }
    };

    [Theory]
    [MemberData(nameof(CloneCases))]
    public void CloneTest(AvgFunction original)
    {
        // Act
        IColumn copie = original.Clone();

        // Assert
        copie.Should()
            .BeOfType<AvgFunction>()
            .Which
            .Should()
            .NotBeSameAs(original)
            .And.Be(original);
    }
}