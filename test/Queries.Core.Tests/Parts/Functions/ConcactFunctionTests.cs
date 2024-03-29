﻿using FluentAssertions;
using Newtonsoft.Json.Linq;
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
[Feature(nameof(ConcatFunction))]
[Feature("Functions")]
public class ConcatFunctionFunctionTests : IDisposable
{
    private ITestOutputHelper _outputHelper;

    public ConcatFunctionFunctionTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

    public void Dispose() => _outputHelper = null;

    public static IEnumerable<object[]> CtorWithNullAsFirstOrSecondArgumentCases
    {
        get
        {
            yield return new object[] { null, null };
            yield return new object[] { 1.Literal(), null };
            yield return new object[] { null, 1.Literal() };
        }
    }

    [Theory]
    [MemberData(nameof(CtorWithNullAsFirstOrSecondArgumentCases))]
    public void CtorThrowsArgumentNullExceptionIfAnyParameterIsNull(IColumn first, IColumn second)
    {
        _outputHelper.WriteLine($"{nameof(first)} : {first}");
        _outputHelper.WriteLine($"{nameof(second)} : {second}");

        // Act
        Action action = () => new ConcatFunction(first, second);

        // Assert
        action.Should().Throw<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void HasFunctionAttribute() => typeof(ConcatFunction).Should()
            .BeDecoratedWithOrInherit<FunctionAttribute>($"{nameof(ConcatFunction)} must be marked with {nameof(FunctionAttribute)}");

    public static IEnumerable<object[]> EqualsCases
    {
        get
        {
            yield return new object[] { Concat("Firstname".Field(), "Lastname".Field()), null, false, "comparing with a null instance" };
            yield return new object[] { Concat("Firstname".Field(), "Lastname".Field()), Concat("Firstname".Field(), "Lastname".Field()), true, "comparing two instances with same columns names and same columns count" };
            yield return new object[] { Concat("Firstname".Field(), "Lastname".Field()), Concat("Lastname".Field(), "Firstname".Field()), false, "comparing two instances with same columns names and same columns count but in different order" };

            {
                ConcatFunction function = Concat("Firstname".Field(), "Lastname".Field());
                yield return new object[] { function, function, true, "comparing instance to itself" };
            }
        }
    }

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualTests(ConcatFunction first, object second, bool expectedResult, string reason)
    {
        _outputHelper.WriteLine($"{nameof(first)} : {first}");
        _outputHelper.WriteLine($"{nameof(second)} : {second}");

        // Act
        bool actualResult = first.Equals(second);

        // Assert
        actualResult.Should()
            .Be(expectedResult, reason);
    }

    public static IEnumerable<object[]> AsTestCases
    {
        get
        {
            yield return new object[]
            {
                new ConcatFunction("firstname".Literal(), " ".Literal(), "lastname".Literal()).As(null),
                null,
            };

            yield return new object[]
            {
                new ConcatFunction("firstname".Literal(), " ".Literal(), "lastname".Literal()).As(string.Empty),
                string.Empty,
            };
        }
    }

    [Theory]
    [MemberData(nameof(AsTestCases))]
    public void SettingAliasTest(ConcatFunction column, string expectedAlias)
        => column.Alias.Should().Be(expectedAlias);
}
