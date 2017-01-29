using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;

namespace Queries.Core.Tests.Columns
{
    public class UpperColumnTest
    {

        [Fact]
        public void ConstructorTestWithNullStringArgument()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var column = new UpperFunction((string)null);
            });
        }

        
        [Fact]
        public void ConstructorTestWithNullColumnArgument()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var column = new UpperFunction((IColumn)null);
            });
        }
        
        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(UpperFunction upperColumn, string expectedAlias)
            => Assert.Equal(expectedAlias, upperColumn.Alias);



        public static IEnumerable<object[]> AsTestCases
        {
            get
            {
                yield return new object[]
                {
                    new UpperFunction("firstname".Field()),
                    null,
                };

                yield return new object[]
                {
                    new UpperFunction("firstname".Field()).As(string.Empty),
                    string.Empty,
                };
            }
        }
    }
}