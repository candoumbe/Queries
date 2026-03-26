using AwesomeAssertions;

using Queries.Core.Attributes;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;

using System;
using System.Collections.Generic;

using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Parts.Functions;

/// <summary>
/// Unit tests for <see cref="NullFunction"/>
/// </summary>
[UnitTest]
[Feature(nameof(NullFunction))]
[Feature("Functions")]
public class NullFunctionTests(ITestOutputHelper outputHelper) : IDisposable
{
    private ITestOutputHelper _outputHelper = outputHelper;

    public void Dispose() => _outputHelper = null;

    public static TheoryData<IColumn, IColumn> CtorThrowsArgumentNullExceptionCases
        => new()
        {
            { null, null },
            { null, "".Literal() },
            { "firstname".Field(), null }
        };

    [Theory]
    [MemberData(nameof(CtorThrowsArgumentNullExceptionCases))]
    public void CtorThrowsArgumentNullExceptionIfAnyParameterIsNull(IColumn column, IColumn defaultValue)
    {
        // Act
        Action action = () => _ = new NullFunction(column, defaultValue);

        // Assert
        action.Should().Throw<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    public static TheoryData<IColumn, IColumn> CtorBuildAValidInstanceCases
        => new()
        {
            { "firstname".Field(), string.Empty.Literal() }
        };

    [Theory]
    [MemberData(nameof(CtorBuildAValidInstanceCases))]
    public void CtorBuildAValidInstance(IColumn column, IColumn defaultValue)
    {
        // Act
        NullFunction function = new(column, defaultValue);

        // Assert
        function.Column.Should().Be(column);
        function.DefaultValue.Should().Be(defaultValue);
    }

    [Fact]
    public void HasFunctionFunctionAttribute()
        => typeof(NullFunction).Should()
        .BeDecoratedWith<FunctionAttribute>($"{nameof(NullFunction)} must be marked with {nameof(FunctionAttribute)}");

    public static TheoryData<NullFunction, object, bool, string> EqualsCases
        => new()
        {
            {
                new NullFunction("nickname".Field(), "Unknown".Literal()),
                null,
                false, $"comparing {nameof(NullFunction)} instance with a null"
            },
            {
                new NullFunction("nickname".Field(), "Unknown".Literal()),
                new NullFunction("nickname".Field(), "Unknown".Literal()),
                true, $"comparing two {nameof(NullFunction)} instances with same inputs"
            },
            {
                new NullFunction("nickname".Field(), 1.Literal()),
                new NullFunction("nickname".Field(), "Unknown".Literal()),
                false, $"comparing two {nameof(NullFunction)} instances with same inputs but different types of {nameof(NullFunction.DefaultValue)}"
            },
            {
                new NullFunction("nickname".Field(), "Unknown".Literal()),
                Select(1.Literal()),
                false, "comparing two different types of query"
            }
        };

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualTests(NullFunction first, object second, bool expectedResult, string reason)
    {
        _outputHelper.WriteLine($"{nameof(first)} : {first}");
        _outputHelper.WriteLine($"{nameof(second)} : {second}");

        // Act
        bool actualResult = first.Equals(second);

        // Assert
        actualResult.Should().Be(expectedResult, reason);
    }
}