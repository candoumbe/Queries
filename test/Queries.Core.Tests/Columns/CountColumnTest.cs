using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Xunit;
using Queries.Core.Parts.Functions;

namespace Queries.Core.Tests.Columns
{
    public class CountColumnTest
    {
        [Fact]
        public void ConstructorTestWithNullArgument()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var countColumn = new CountFunction(null);
            });
        }

        public static IEnumerable<object[]> AsTestCases
        {
            get
            {
                yield return new object[]
                {
                    new CountFunction("firstname".Field()),
                    null,
                };

                yield return new object[]
                {
                    new CountFunction("firstname".Field()).As(string.Empty),
                    string.Empty,
                };
            }
        }

        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(CountFunction concatColumn, string expectedAlias)
            => Assert.Equal(expectedAlias, concatColumn.Alias);
    }
}