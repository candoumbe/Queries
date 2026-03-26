#if NET6_0_OR_GREATER

namespace Queries.Core.Tests.Parts.Columns;

using AwesomeAssertions;

using Queries.Core.Parts.Columns;

using System;
using System.Collections.Generic;

using Xunit;
using Xunit.Categories;

[UnitTest]
[Feature("Column")]
[Feature(nameof(TimeColumn))]
public class TimeColumnTests
{
    public static TheoryData<TimeOnly, string> ConstructorWithFormatCases
        => new()
        {
            { TimeOnly.MinValue, null },
            { TimeOnly.MinValue, "" },
            { TimeOnly.MaxValue, "" },
            { TimeOnly.MaxValue, null }
        };

    [Theory]
    [MemberData(nameof(ConstructorWithFormatCases))]
    public void CtorWithFormatBuildsAValidInstance(TimeOnly columnValue, string format)
    {
        // Act
        TimeColumn dc = new (columnValue, format);

        // Assert
        dc.Alias.Should().BeNull();
        dc.Value.Should().Be(columnValue);
        dc.StringFormat.Should().Be(format);
    }

    public static TheoryData<TimeOnly> ConstructorWithoutFormatCases
        => new()
        {
            { TimeOnly.MinValue },
            { TimeOnly.MaxValue }
        };

    [Theory]
    [MemberData(nameof(ConstructorWithoutFormatCases))]
    public void CtorWithoutFormatBuildsAValidInstance(TimeOnly columnValue)
    {
        // Act
        TimeColumn dc = new (columnValue);

        // Assert
        dc.Alias.Should().BeNull();
        dc.Value.Should().Be(columnValue);
        dc.StringFormat.Should().Be("HH:mm:ss.FFFFFFF");
    }
}

#endif