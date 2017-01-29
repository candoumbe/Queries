using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;
using FluentAssertions;

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
                    new ConcatFunction("firstname".Literal(), " ".Literal(), "lastname".Literal()).As(null),
                    null,
                };

                yield return new object[]
                {
                    new ConcatFunction("firstname".Literal(), " ".Literal(), "lastname".Literal()).As(string.Empty),
                    string.Empty, 
                };
            }
        }

        [Fact]
        public void ConstructorWithNoArgument()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var column = new ConcatFunction();
            });
        }

        [Fact]
        public void ConstructorWithOneColumn_Should_Throw_ArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var column = new ConcatFunction(new LiteralColumn());
            });
        }


        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(ConcatFunction concatColumn, string expectedAlias)
            => concatColumn.Alias.Should().Be(expectedAlias);

    }
}