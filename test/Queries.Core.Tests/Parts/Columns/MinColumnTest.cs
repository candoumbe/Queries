using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;
using FluentAssertions;

namespace Queries.Core.Tests.Parts.Columns
{
    public class MinColumnTest
    {
        [Fact]
        public void ConstructorTestWithNullStringArgument()
        {
            Action action = () => new MinFunction((string)null);

            action.ShouldThrowExactly<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ConstructorTestWithEmptyStringArgument()
        {
            Action action = () => new MinFunction(string.Empty);

            action.ShouldThrowExactly<ArgumentOutOfRangeException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ConstructorTestWithWhitespaceStringArgument()
        {
            Action action = () => new MinFunction("   ");

            action.ShouldThrowExactly<ArgumentOutOfRangeException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ConstructorTestWithNullColumnArgument()
        {
            Action action = () => new MinFunction((IColumn) null);

            action.ShouldThrowExactly<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ConstructorTestColumnArgument() => new MinFunction("age").Type.Should().Be(AggregateType.Min);
        
        
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
        public void SettingAliasTest(MinFunction column, string expectedAlias)
            => column.Alias.Should().Be(expectedAlias);
    }
}