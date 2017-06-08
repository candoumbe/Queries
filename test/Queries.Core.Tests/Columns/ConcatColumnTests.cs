using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;
using FluentAssertions;

namespace Queries.Core.Tests.Columns
{
    public class ConcatColumnTests
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


        public static IEnumerable<object[]> ConstructorThrowsArgumentNullExceptionCases
        {
            get
            {
                yield return new object[] { (IColumn)null, null };
                yield return new object[] { new LiteralColumn("Firstname"), null };
                yield return new object[] { null,  new LiteralColumn("Lastname")};
            }
        }

        [Theory]
        [MemberData(nameof(ConstructorThrowsArgumentNullExceptionCases))]
        public void ConstructorWithOneColumn_Should_Throw_ArgumentOutOfRangeException(IColumn first, IColumn second)
        {
            Action action = () => new ConcatFunction(first, second);

            action.ShouldThrowExactly<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }


        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(ConcatFunction column, string expectedAlias)
            => column.Alias.Should().Be(expectedAlias);

    }
}