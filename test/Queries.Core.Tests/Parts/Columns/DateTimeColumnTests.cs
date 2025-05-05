using FluentAssertions;
using Queries.Core.Parts.Columns;
using System;
using Xunit;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Columns;

[UnitTest]
[Feature("Column")]
[Feature(nameof(DateTimeColumn))]
public class DateTimeColumnTests
{
    public static TheoryData<DateTime, string> ConstructorWithFormatCases => new()
    {
        { DateTime.MinValue, null },
        { DateTime.MinValue, "" },
        { DateTime.MaxValue, "" },
        { DateTime.MaxValue, null }
    };

    [Theory]
    [MemberData(nameof(ConstructorWithFormatCases))]
    public void CtorWithFormatBuildsAValidInstance(DateTime columnValue, string format)
    {
        // Act
        DateTimeColumn dc = new(columnValue, format);

        // Assert
        dc.Alias.Should().BeNull();
        dc.Value.Should().Be(columnValue);
        dc.StringFormat.Should().Be(format);
    }

    public static TheoryData<DateTime> ConstructorWithoutFormatCases => new()
    {
        DateTime.MinValue,
        DateTime.MaxValue
    };

    [Theory]
    [MemberData(nameof(ConstructorWithoutFormatCases))]
    public void CtorWithoutFormatBuildsAValidInstance(DateTime columnValue)
    {
        // Act
        DateTimeColumn dc = new(columnValue);

        // Assert
        dc.Alias.Should().BeNull();
        dc.Value.Should().Be(columnValue);
        dc.StringFormat.Should().Be("yyyy-MM-dd");
    }
}