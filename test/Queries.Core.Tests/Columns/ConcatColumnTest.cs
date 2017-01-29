using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;

namespace Queries.Core.Tests.Columns
{
    public class ConcatColumnTest
    {

        public static IEnumerable<object[]> AsTestCases
        {
            get
            {
                yield return new object[]
                {
                    new ConcatColumn("firstname".Literal(), " ".Literal(), "lastname".Literal()).As(null),
                    null,
                };

                yield return new object[]
                {
                    new ConcatColumn("firstname".Literal(), " ".Literal(), "lastname".Literal()).As(string.Empty),
                    string.Empty, 
                };
            }
        }

        [Fact]
        public void ConstructorWithNoArgument()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var column = new ConcatColumn();
            });
        }

        [Fact]
        public void ConstructorWithOneColumn()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var column = new ConcatColumn(new LiteralColumn());
            });
        }


        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(ConcatColumn concatColumn, string expectedAlias)
            => Assert.Equal(expectedAlias, concatColumn.Alias);

    }
}