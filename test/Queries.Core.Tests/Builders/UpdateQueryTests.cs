using AwesomeAssertions;
using Queries.Core.Attributes;
using Queries.Core.Builders;
using Queries.Core.Parts;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Builders;

[UnitTest]
[Feature("Update")]
[Feature("Builder")]
public class UpdateQueryTests(ITestOutputHelper outputHelper)
{
    [Fact]
    public void CtorWithNullStringArgumentThrowsArgumentNullException()
    {
        // Act
        Action action = () => _ = new UpdateQuery((string) null);

        // Assert
        action.Should().Throw<ArgumentNullException>("name of the table to delete cannot be null").Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void CtorWithNullTableArgumentThrowsArgumentNullException()
    {
        // Act
        Action action = () => _ = new UpdateQuery((Table)null);

        // Assert
        action.Should().Throw<ArgumentNullException>("name of the table to delete cannot be null").Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void CtorWithEmptyOrWhiteSpaceArgumentThrowsArgumentOutOfRangeException(string tableName)
    {
        // Act
        Action action = () => _ = new UpdateQuery(tableName);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>("name of the table to delete cannot be null").Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    public static TheoryData<UpdateQuery, object, bool, string> EqualsCases => new()
    {
        { Update("SuperHero").Set("firstname".Field().UpdateValueTo("Bruce")), null, false, "comparing with a null instance" },
        { Update("SuperHero").Set("firstname".Field().UpdateValueTo("Bruce")), Update("SuperHero").Set("firstname".Field().UpdateValueTo("Bruce")), true, "comparing two instances with same tableName" },
        { Update("SuperHero").Set("firstname".Field().UpdateValueTo("Bruce")), Update("SuperHero").Set("firstname".Field().UpdateValueTo("Bruce")), true, "comparing two instances with same tableName" },
        { Update("SuperHero"), Select(1.Literal()), false, "comparing two different types of query" }
    };

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualTests(UpdateQuery first, object second, bool expectedResult, string reason)
    {
        outputHelper.WriteLine($"{nameof(first)} : {first}");
        outputHelper.WriteLine($"{nameof(second)} : {second}");

        // Act
        bool actualResult = first.Equals(second);

        // Assert
        actualResult.Should().Be(expectedResult, reason);
    }

    [Fact]
    public void IsDataManipulationQuery()
    {
        // Arrange
        TypeInfo typeInfo = typeof(UpdateQuery)
            .GetTypeInfo();

        // Act
        Attribute attr = typeInfo.GetCustomAttribute<DataManipulationLanguageAttribute>();

        // Assert
        attr.Should()
            .NotBeNull($"{nameof(UpdateQuery)} class must be marked with {nameof(DataManipulationLanguageAttribute)}");
    }
}