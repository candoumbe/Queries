using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;

namespace Queries.Core.Tests.Columns
{
    public class MinColumnTest
    {
        [Fact]
        public void ConstructorTestWithNullStringArgument()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var column = new MinFunction((string)null);
            });
        }

        [Fact]
        public void ConstructorTestWithEmptyStringArgument()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var column = new MinFunction(string.Empty);
            });
        }

        [Fact]
        public void ConstructorTestWithWhitespaceStringArgument()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var column = new MinFunction(" ");
            });
        }

        [Fact]
        public void ConstructorTestWithNullColumnArgument()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var column = new MinFunction((IColumn)null);
            });
        }

        [Fact]
        public void ConstructorTestColumnArgument()
        {
            Assert.Equal(AggregateType.Min, new MinFunction("age").Type);
        }

        public static IEnumerable<object[]> AsTestCases
        {
            get
            {
                yield return new object[]
                {
                    new MinFunction("age".Field()),
                    null,
                };

                yield return new object[]
                {
                    new MinFunction("age".Field()).As(string.Empty),
                    string.Empty,
                };
            }
        }

        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(MinFunction concatColumn, string expectedAlias)
            => Assert.Equal(expectedAlias, concatColumn.Alias);
    }
}