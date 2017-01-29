using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;

namespace Queries.Core.Tests.Columns
{
    public class AvgColumnTest
    {
        [Fact]
        public void ConstructorTestWithNullStringArgument()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var column = new AvgFunction((string)null);
            });
        }

        [Fact]
        public void ConstructorTestWithEmptyStringArgument()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var column = new AvgFunction(string.Empty);
            });
        }

        [Fact]
        public void ConstructorTestWithWhitespaceStringArgument()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var column = new AvgFunction(" ");
            });
        }

        [Fact]
        public void ConstructorTestWithNullColumnArgument()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var column = new AvgFunction((IColumn)null);
            });
        }

        [Fact]
        public void ConstructorTestColumnArgument()
        {
            Assert.Equal(AggregateType.Average, new AvgFunction("age").Type);
        }

        public static IEnumerable<object[]> AsTestCases
        {
            get
            {
                yield return new object[]
                {
                    new AvgFunction("age".Field()),
                    null,
                };

                yield return new object[]
                {
                    new AvgFunction("age".Field()).As(string.Empty),
                    string.Empty,
                };
            }
        }

        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(AvgFunction concatColumn, string expectedAlias)
            => Assert.Equal(expectedAlias, concatColumn.Alias);
    }
}