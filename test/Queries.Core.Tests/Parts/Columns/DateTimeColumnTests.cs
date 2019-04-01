using FluentAssertions;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Columns
{
    [UnitTest]
    [Feature("Column")]
    [Feature(nameof(DateTimeColumn))]
    public class DateTimeColumnTests
    {
        public static IEnumerable<object[]> ConstructorWithFormatCases
        {
            get
            {
                yield return new object[] { DateTime.MinValue, null };
                yield return new object[] { DateTime.MinValue, "" };
                yield return new object[] { DateTime.MaxValue, "" };
                yield return new object[] { DateTime.MaxValue, null };
            }
        }

        [Theory]
        [MemberData(nameof(ConstructorWithFormatCases))]
        public void CtorWithFormatBuildsAValidInstance(DateTime columnValue, string format)
        {
            // Act
            DateTimeColumn dc = new DateTimeColumn(columnValue, format);

            // Assert
            dc.Alias.Should().BeNull();
            dc.Value.Should().Be(columnValue);
            dc.StringFormat.Should().Be(format);
        }

        public static IEnumerable<object[]> ConstructorWithoutFormatCases
        {
            get
            {
                yield return new object[] { DateTime.MinValue};
                yield return new object[] { DateTime.MaxValue};
            }
        }

        [Theory]
        [MemberData(nameof(ConstructorWithoutFormatCases))]
        public void CtorWithoutFormatBuildsAValidInstance(DateTime columnValue)
        {
            // Act
            DateTimeColumn dc = new DateTimeColumn(columnValue);

            // Assert
            dc.Alias.Should().BeNull();
            dc.Value.Should().Be(columnValue);
            dc.StringFormat.Should().Be("yyyy-MM-dd");
        }
    }
}
