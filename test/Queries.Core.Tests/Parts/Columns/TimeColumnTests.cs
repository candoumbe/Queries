#if NET6_0_OR_GREATER

namespace Queries.Core.Tests.Parts.Columns;

using FluentAssertions;

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
    public static IEnumerable<object[]> ConstructorWithFormatCases
    {
        get
        {
            yield return new object[] { TimeOnly.MinValue, null };
            yield return new object[] { TimeOnly.MinValue, "" };
            yield return new object[] { TimeOnly.MaxValue, "" };
            yield return new object[] { TimeOnly.MaxValue, null };
        }
    }

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

    public static IEnumerable<object[]> ConstructorWithoutFormatCases
    {
        get
        {
            yield return new object[] { TimeOnly.MinValue };
            yield return new object[] { TimeOnly.MaxValue };
        }
    }

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