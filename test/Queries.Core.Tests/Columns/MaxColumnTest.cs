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
                var column = new MaxFunction((string)null);
            });
        }

        [Fact]
        public void ConstructorTestWithEmptyStringArgument()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var column = new MaxFunction(string.Empty);
            });
        }

        [Fact]
        public void ConstructorTestWithWhitespaceStringArgument()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var column = new MaxFunction(" ");
            });
        }

        [Fact]
        public void ConstructorTestWithNullColumnArgument()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var column = new MaxFunction((IColumn)null);
            });
        }

        [Fact]
        public void ConstructorTestColumnArgument()
        {
            Assert.Equal(AggregateType.Max, new MaxFunction("age").Type);
        }

        public static IEnumerable<object[]> AsTestCases
        {
            get
            {
                yield return new object[]
                {
                    new MaxFunction("age".Field()),
                    null,
                };

                yield return new object[]
                {
                    new MaxFunction("age".Field()).As(string.Empty),
                    string.Empty,
                };
            }
        }

        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(MaxFunction column, string expectedAlias)
            => Assert.Equal(expectedAlias, column.Alias);
    }
}