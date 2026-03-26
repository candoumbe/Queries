using AwesomeAssertions;
using Queries.Core.Parts.Columns;
using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Columns;

[UnitTest]
[Feature(nameof(Literal))]
[Feature("Column")]
public class LiteralTests(ITestOutputHelper outputHelper)
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("newAlias")]
    public void SettingAlias(string newAlias)
    {
        // Arrange
        Literal column = "column".Literal().As(newAlias);

        // Act
        column = column.As(newAlias);

        // Assert
        column.Alias.Should().Be(newAlias);
    }

    public static TheoryData<Literal, object, bool, string> EqualsCases
        => new()
        {
            { "firstname".Literal(), null, false, "object is null" },
            { "firstname".Literal(), "firstname".Literal(), true, $"object is a {nameof(Literal)} with exactly the same {nameof(Literal.Value)} and {nameof(Literal.Alias)}" },
            { "firstname".Literal(), "firstname".Literal(), true, "Equals with same instance" }
        };

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualTests(Literal first, object second, bool expectedResult, string reason)
    {
        outputHelper.WriteLine($"{nameof(first)} : {first}");
        outputHelper.WriteLine($"{nameof(second)} : {second}");

        // Act
        bool actualResult = first.Equals(second);

        // Assert
        actualResult.Should().Be(expectedResult, reason);
    }

    public static TheoryData<Literal> CloneCases
        => new()
        {
            { 1.Literal() },
            { "Bruce".Literal() }
        };

    [Theory]
    [MemberData(nameof(CloneCases))]
    public void CloneTest(Literal original)
    {
        // Act
        IColumn copie = original.Clone();

        // Assert
        copie.Should()
            .BeAssignableTo<Literal>().Which.Should()
            .NotBeSameAs(original).And
            .Be(original);
    }
}