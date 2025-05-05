using System;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;
using FluentAssertions;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Columns;

[UnitTest]
[Feature(nameof(FieldColumn))]
public class FieldColumnTests(ITestOutputHelper outputHelper)
{
    [Fact]
    public void ConstructorTestWithNullArgument()
    {
        // Act
        Action action = () => _ = new FieldColumn(null);

        // Arrange
        action.Should().Throw<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ConstructorTestWithEmptyArgument()
    {
        // Act
        Action action = () => _ = new FieldColumn(string.Empty);

        // Arrange
        action.Should().Throw<ArgumentOutOfRangeException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ConstructorTestWithWhitespaceStringArgument()
    {
        // Act
        Action action = () => _ = new FieldColumn("   ");

        // Arrange
        action.Should().Throw<ArgumentOutOfRangeException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    public static TheoryData<AggregateFunction, string> AsTestCases => new()
    {
        { new CountFunction("firstname".Field()), null },
        { new CountFunction("firstname".Field()).As(string.Empty), string.Empty }
    };

    [Theory]
    [MemberData(nameof(AsTestCases))]
    public void SettingAliasTest(CountFunction column, string expectedAlias)
        => column.Alias.Should().Be(expectedAlias);

    public static TheoryData<FieldColumn, object, bool, string> EqualsCases
    {
        get
        {
            TheoryData<FieldColumn, object, bool, string> data = new()
            {
                { new FieldColumn("firstname"), null, false, "object is null" },
                { new FieldColumn("firstname"), new FieldColumn("firstname"), true, $"object is a {nameof(FieldColumn)} with exactly the same {nameof(FieldColumn.Name)} and {nameof(FieldColumn.Alias)}" }
            };

            {
                FieldColumn column = new("firstname");
                data.Add(column, column, true, "Equals with same instance");
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualTests(FieldColumn first, object second, bool expectedResult, string reason)
    {
        outputHelper.WriteLine($"First : {first}");
        outputHelper.WriteLine($"Second : {second}");

        // Act
        bool actualResult = first.Equals(second);

        // Assert
        actualResult.Should().Be(expectedResult, reason);
    }

    public static TheoryData<FieldColumn> CloneCases => new()
    {
        { "Firstname".Field() }
    };
    [Theory]
    [MemberData(nameof(CloneCases))]
    public void CloneTest(FieldColumn original)
    {
        // Act
        FieldColumn copie = original.Clone();

        // Assert
        copie.Should()
            .NotBeSameAs(original).And
            .Be(original);
    }
}