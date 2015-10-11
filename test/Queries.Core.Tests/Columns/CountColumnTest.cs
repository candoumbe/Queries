using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Xunit;

namespace Queries.Core.Tests.Columns
{
    public class CountColumnTest
    {
        [Fact]
        public void ConstructorTestWithNullArgument()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var countColumn = new CountColumn(null);
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