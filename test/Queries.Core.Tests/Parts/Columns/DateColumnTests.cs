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
[Feature(nameof(DateColumn))]
public class DateColumnTests
{
    public static IEnumerable<object[]> ConstructorWithFormatCases
    {
        get
        {
            yield return new object[] { DateOnly.MinValue, null };
            yield return new object[] { DateOnly.MinValue, "" };
            yield return new object[] { DateOnly.MaxValue, "" };
            yield return new object[] { DateOnly.MaxValue, null };
        }
    }

    [Theory]
    [MemberData(nameof(ConstructorWithFormatCases))]
    public void CtorWithFormatBuildsAValidInstance(DateOnly columnValue, string format)
    {
        // Act
        DateColumn dc = new (columnValue, format);

        // Assert
        dc.Alias.Should().BeNull();
        dc.Value.Should().Be(columnValue);
        dc.StringFormat.Should().Be(format);
    }

    public static IEnumerable<object[]> ConstructorWithoutFormatCases
    {
        get
        {
            yield return new object[] { DateOnly.MinValue };
            yield return new object[] { DateOnly.MaxValue };
        }
    }

    [Theory]
    [MemberData(nameof(ConstructorWithoutFormatCases))]
    public void CtorWithoutFormatBuildsAValidInstance(DateOnly columnValue)
    {
        // Act
        DateColumn dc = new (columnValue);

        // Assert
        dc.Alias.Should().BeNull();
        dc.Value.Should().Be(columnValue);
        dc.StringFormat.Should().Be("yyyy-MM-dd");
    }
}

#endif