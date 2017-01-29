using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;

namespace Queries.Core.Tests.Functions
{
    public class MaxColumnTest
    {
        [Fact]
        public void ConstructorTestWithNullStringArgument()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var column = new MaxColumn((string)null);
            });
        }

        [Fact]
        public void ConstructorTestWithEmptyStringArgument()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var column = new MaxColumn(string.Empty);
            });
        }

        [Fact]
        public void ConstructorTestWithWhitespaceStringArgument()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var column = new MaxColumn(" ");
            });
        }

        [Fact]
        public void ConstructorTestWithNullColumnArgument()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var column = new MaxColumn((IColumn)null);
            });
        }

        [Fact]
        public void ConstructorTestColumnArgument()
        {
            Assert.Equal(AggregateType.Max, new MaxColumn("age").Type);
        }

        public static IEnumerable<object[]> AsTestCases
        {
            get
            {
                yield return new object[]
                {
                    new MaxColumn("age".Field()),
                    null,
                };

                yield return new object[]
                {
                    new MaxColumn("age".Field()).As(string.Empty),
                    string.Empty,
                };
            }
        }

        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(MaxColumn column, string expectedAlias)
            => Assert.Equal(expectedAlias, column.Alias);
    }
}