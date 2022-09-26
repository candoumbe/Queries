using FluentAssertions;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using Queries.Core.Attributes;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Functions;

[UnitTest]
[Feature(nameof(AvgFunction))]
[Feature("Functions")]
public class AvgFunctionTests : IDisposable
{
    private ITestOutputHelper _outputHelper;

    public AvgFunctionTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

    public void Dispose() => _outputHelper = null;

    [Fact]
    public void CtorThrowsArgumentNullExceptionIfColumnParameterIsNull()
    {
        // Act
        Action action = () => new AvgFunction((IColumn) null);

        // Assert
        action.Should().Throw<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void HasFunctionAttribute() => typeof(AvgFunction).Should()
        .BeDecoratedWithOrInherit<FunctionAttribute>($"{nameof(AvgFunction)} must be marked with {nameof(FunctionAttribute)}");

    public static IEnumerable<object[]> EqualsCases
    {
        get
        {
            yield return new object[] { Avg("Age".Field()), null, false, $"comparing {nameof(AvgFunction)} with a null instance" };
            yield return new object[] { Avg("Age".Field()), Avg("Age".Field()), true, $"comparing two {nameof(AvgFunction)} instances with same column names" };

            {
                AvgFunction function = Avg("Age".Field());
                yield return new object[] { function, function, true, $"comparing {nameof(AvgFunction)} instance to itself" };
            }
        }
    }

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualTests(AvgFunction first, object second, bool expectedResult, string reason)
    {
        _outputHelper.WriteLine($"{nameof(first)} : {first}");
        _outputHelper.WriteLine($"{nameof(second)} : {second}");

        // Act
        bool actualResult = first.Equals(second);

        // Assert
        actualResult.Should().Be(expectedResult, reason);
    }

    [Fact]
    public void ConstructorTestWithNullStringArgument()
    {
        Action action = () => new AvgFunction((string)null);

        action.Should()
            .ThrowExactly<ArgumentNullException>().Which
            .ParamName.Should()
                .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ConstructorTestWithEmptyStringArgument()
    {
        Action action = () => new AvgFunction(string.Empty);

        action.Should()
            .ThrowExactly<ArgumentOutOfRangeException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ConstructorTestWithWhitespaceStringArgument()
    {
        Action action = () => new AvgFunction("   ");

        action.Should()
            .ThrowExactly<ArgumentOutOfRangeException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ConstructorTestWithNullColumnArgument()
    {
        Action action = () => new AvgFunction((IColumn)null);

        action.Should()
            .ThrowExactly<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ConstructorTestColumnArgument() => new AvgFunction("age").Type
        .Should().Be(AggregateType.Average);

    public static IEnumerable<object[]> AsTestCases
    {
        get
        {
            yield return new object[]
            {
                new AvgFunction("age".Field()),
                null,
            };

            yield return new object[]
            {
                new AvgFunction("age".Field()).As(string.Empty),
                string.Empty,
            };
        }
    }

    [Theory]
    [MemberData(nameof(AsTestCases))]
    public void SettingAliasTest(AvgFunction column, string expectedAlias)
        => column.Alias.Should().Be(expectedAlias);

    public static IEnumerable<object[]> CloneCases
    {
        get
        {
            yield return new[] { new AvgFunction("Firstname") };
            yield return new[] { new AvgFunction("Firstname".Field()) };
        }
    }

    [Theory]
    [MemberData(nameof(CloneCases))]
    public void CloneTest(AvgFunction original)
    {
        // Act
        IColumn copie = original.Clone();

        // Assert
        copie.Should()
            .BeOfType<AvgFunction>().Which.Should()
            .NotBeSameAs(original).And
            .Be(original);
    }
}
