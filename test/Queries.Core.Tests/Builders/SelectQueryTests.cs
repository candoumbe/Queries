using AwesomeAssertions;
using Queries.Core.Builders;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using Queries.Core.Builders.Fluent;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Builders;

/// <summary>
/// Unit tests for <see cref="SelectQuery"/>.
/// </summary>
[UnitTest]
[Feature("Select")]
[Feature("Builder")]
public class SelectQueryTests : IDisposable
{
    private ITestOutputHelper _outputHelper;

    public SelectQueryTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

    public void Dispose() => _outputHelper = null;

    public static TheoryData<SelectQuery, object, bool, string> EqualsCases => new()
    {
        { Select("Firstname"), null, false, "comparing with a null instance" },
        { Select("Firstname"), Select("Firstname"), true, "comparing two instances with same columns names and same columns count" },
        { Select(1.Literal()), Select(1.Literal()), true, "comparing two instances with same columns" },
        {
            Select(Null("RealValue".Field(), "TextValue".Field()))
                .From("Parameter")
                .Where("ParameterName".Field().EqualTo("p0"))
                .OrderBy(1.Literal().Desc())
                .Paginate(pageIndex: 1, 1),
    
            Select(Null("RealValue".Field(), "TextValue".Field()))
                .From("Parameter")
                .Where("ParameterName".Field().EqualTo("p0"))
                .OrderBy(1.Literal().Desc())
                .Paginate(pageIndex: 1, 1),
            true,
            "Two differents instances of the same query"
        },
        {
            Select("Firstname".Field(), "Lastname".Field())
            .From("People")
            .OrderBy(1.Literal().Desc())
            .Paginate(pageIndex:1, pageSize:1),
    
            Select("Firstname".Field(), "Lastname".Field())
            .From("People")
            .OrderBy(1.Literal().Desc())
            .Paginate(pageIndex:1, pageSize:1),
            true,
            "Two select queries with pagination"
        }
    };

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualTests(SelectQuery first, object second, bool expectedResult, string reason)
    {
        _outputHelper.WriteLine($"{nameof(first)} : {first}");
        _outputHelper.WriteLine($"{nameof(second)} : {second}");

        // Act
        bool actualResult = first.Equals(second);

        // Assert
        actualResult.Should().Be(expectedResult, reason);
    }

    public static TheoryData<IBuild<SelectQuery>> CloneCases => new()
    {
        Select(1.Literal()),
        Select("*").From(
            Select("Firstname".Field(), "Lastname".Field()).From("People")
                .Union(
                    Select("Username".Field(), "Nickname".Field()).From("SuperHeroes"))),
        Select("Firstname".Field(), "Lastname".Field())
            .From("People")
            .OrderBy(1.Literal().Desc())
            .Paginate(pageIndex:1, pageSize:1)
    };

    [Theory]
    [MemberData(nameof(CloneCases))]
    public void CloneTest(SelectQuery original)
    {
        _outputHelper.WriteLine($"{nameof(original)} : {original}");

        // Act
        SelectQuery copy = original.Clone();

        // Assert
        copy.Should()
            .NotBeSameAs(original).And
            .Be(original);
    }

    public static TheoryData<IEnumerable<IColumn>, string> CtorThrowsArgumentOutOfRangeExceptionCases => new()
    {
        { [], $"empty array of {nameof(IColumn)}s" },
        { Enumerable.Repeat<IColumn>(null, 5), $"array of 5 null {nameof(IColumn)}s" }
    };

    [Theory]
    [MemberData(nameof(CtorThrowsArgumentOutOfRangeExceptionCases))]
    public void CtorThrowsArgumentOutOfRangeException(IEnumerable<IColumn> columns, string reason)
    {
        // Act
        Action action = () => _ = new SelectQuery(columns.ToArray());

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>(reason).Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }
}