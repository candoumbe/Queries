using FluentAssertions;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Columns;

[UnitTest]
[Feature(nameof(StringColumn))]
[Feature(nameof(Columns))]
public class StringColumnTests : IDisposable
{
    private ITestOutputHelper _outputHelper;

    public StringColumnTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    public void Dispose() => _outputHelper = null;

    [Fact]
    public void CtorShouldSetValueAsPassedIn()
    {
        new StringColumn(null).Value.Should().BeNull($"new {nameof(StringColumn)}(null).Value should be null");

        new StringColumn(string.Empty).Value.Should().Be(string.Empty, $"new {nameof(StringColumn)}(string.Empty).Value should be null");
    }

    public static IEnumerable<object[]> EqualsCases
    {
        get
        {
            yield return new object[] { new StringColumn("a"), null, false, "object is null" };
            yield return new object[] { new StringColumn("a"), new StringColumn("a"), true, $"object is a {nameof(StringColumn)} with exactly the same {nameof(StringColumn.Value)} and {nameof(StringColumn.Alias)}" };

            {
                StringColumn column = new("a");
                yield return new object[] { column, column, true, "Equals with same instance" };
            }
        }
    }

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualTests(StringColumn first, object second, bool expectedResult, string reason)
    {
        _outputHelper.WriteLine($"{nameof(first)} : {first}");
        _outputHelper.WriteLine($"{nameof(second)} : {second}");

        // Act
        bool actualResult = first.Equals(second);

        // Assert
        actualResult.Should().Be(expectedResult, reason);
    }

    [Fact]
    public void CloneTest()
    {
        // Arrange
        StringColumn original = new("a");
        _outputHelper.WriteLine($"{nameof(original)} : {original}");

        // Act
        IColumn clone = original.Clone();

        // Assert
        clone.Should()
            .BeOfType<StringColumn>().Which.Should()
            .NotBeSameAs(original).And
            .Be(original);
    }
}
