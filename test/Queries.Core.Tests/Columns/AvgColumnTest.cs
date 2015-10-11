using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Xunit;

namespace Queries.Core.Tests.Columns
{
    public class AvgColumnTest
    {
        [Fact]
        public void ConstructorTestWithNullStringArgument()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var column = new AvgColumn((string)null);
            });
        }

        [Fact]
        public void ConstructorTestWithEmptyStringArgument()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var column = new AvgColumn(string.Empty);
            });
        }

        [Fact]
        public void ConstructorTestWithWhitespaceStringArgument()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var column = new AvgColumn(" ");
            });
        }

        [Fact]
        public void ConstructorTestWithNullColumnArgument()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var column = new AvgColumn((IColumn)null);
            });
        }

        [Fact]
        public void ConstructorTestColumnArgument()
        {
            Assert.Equal(AggregateType.Average, new AvgColumn("age").Type);
        }

        public static IEnumerable<object[]> AsTestCases
        {
            get
            {
                yield return new object[]
                {
                    new AvgColumn("age".Field()),
                    null,
                };

                yield return new object[]
                {
                    new AvgColumn("age".Field()).As(string.Empty),
                    string.Empty,
                };
            }
        }

        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(AvgColumn concatColumn, string expectedAlias)
            => Assert.Equal(expectedAlias, concatColumn.Alias);
    }
}