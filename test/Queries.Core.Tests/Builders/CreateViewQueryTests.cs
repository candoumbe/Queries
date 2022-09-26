using FluentAssertions;

using Queries.Core.Attributes;
using Queries.Core.Builders;

using System;
using System.Collections.Generic;
using System.Reflection;

using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Builders;

[UnitTest]
[Feature("Create view")]
[Feature("Builder")]
public class CreateViewQueryTests : IDisposable
{
    private ITestOutputHelper _outputHelper;

    public CreateViewQueryTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

    public void Dispose() => _outputHelper = null;

    [Fact]
    public void CtorThrowsArgumentNullException()
    {
        // Act 
        Action action = () => new CreateViewQuery(null);

        // Assert
        action.Should().Throw<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void CtorThrowsArgumentOutOfRangeException(string viewName)
    {
        // Act 
        Action action = () => new CreateViewQuery(viewName);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>("viewName is empty or whitespace")
            .Where( ex => !string.IsNullOrWhiteSpace(ex.ParamName));
    }

    public static IEnumerable<object[]> EqualsCases
    {
        get
        {
            yield return new object[] { CreateView("firstname"), null, false, "object is null" };
            yield return new object[] {
                CreateView("SuperHero")
                    .As(
                        Select("Nickname".Field())
                        .From("DC_Comics")
                        .Build())
                        .Build(),
                CreateView("SuperHero")
                    .As(
                        Select("Nickname".Field())
                        .From("DC_Comics")
                        .Build())
                        .Build(),
                true,
                $"object is a {nameof(CreateView)} with exactly the same {nameof(CreateViewQuery.ViewName)} and {nameof(CreateViewQuery.SelectQuery)}" };

            {
                CreateViewQuery query = CreateView("SuperHero")
                    .As(
                        Select(Concat("Firstname".Field(), "Lastname".Field()))
                        .From("DC_Comics")
                        .Build())
                        .Build();
                yield return new object[] { query, query, true, "Equals with same instance" };
            }
        }
    }

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualTests(CreateViewQuery first, object second, bool expectedResult, string reason)
    {
        _outputHelper.WriteLine($"{nameof(first)} : {first}");
        _outputHelper.WriteLine($"{nameof(second)} : {second}");

        // Act
        bool actualResult = first.Equals(second);

        // Assert
        actualResult.Should().Be(expectedResult, reason);
    }

    [Fact]
    public void IsMarkedWithDataDefinitionQueryAttribute()
    {
        // Arrange
        TypeInfo typeInfo = typeof(CreateViewQuery).GetTypeInfo();

        // Act
        DataManipulationLanguageAttribute attr = typeInfo.GetCustomAttribute<DataManipulationLanguageAttribute>();

        // Arrange
        attr.Should()
            .NotBeNull($"{nameof(CreateViewQuery)} must be marked with {nameof(DataManipulationLanguageAttribute)}");
    }
}
