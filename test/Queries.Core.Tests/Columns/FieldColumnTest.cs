using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;

namespace Queries.Core.Tests.Columns
{
    public class FieldColumnTest
    {
        [Fact]
        public void ConstructorTestWithNullArgument()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var column = new FieldColumn(null);
            });
        }

        [Fact]
        public void ConstructorTestWithEmptyArgument()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var column = new FieldColumn(string.Empty);
            });
        }

        [Fact]
        public void ConstructorTestWithWhitespaceStringArgument()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var column = new FieldColumn("  ");
            });
        }

        public static IEnumerable<object[]> AsTestCases
        {
            get
            {
                yield return new object[]
                {
                    new CountColumn("firstname".Field()),
                    null,
                };

                yield return new object[]
                {
                    new CountColumn("firstname".Field()).As(string.Empty),
                    string.Empty,
                };
            }
        }

        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(CountColumn concatColumn, string expectedAlias)
            => Assert.Equal(expectedAlias, concatColumn.Alias);
    }
}