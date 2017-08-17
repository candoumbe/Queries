using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;
using FluentAssertions;

namespace Queries.Core.Tests.Parts.Columns
{
    public class UpperColumnTest
    {

        [Fact]
        public void ConstructorTestWithNullStringArgument()
        {
            Action action = () => new UpperFunction((string)null);

            action.ShouldThrowExactly<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        
        [Fact]
        public void ConstructorTestWithNullColumnArgument()
        {
            Action action = () => new UpperFunction((IColumn) null);

            action.ShouldThrowExactly<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }
        
        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(UpperFunction column, string expectedAlias)
            => column.Alias.Should().Be(expectedAlias);



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