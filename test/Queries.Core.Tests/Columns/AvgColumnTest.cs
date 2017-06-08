using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;
using FluentAssertions;
namespace Queries.Core.Tests.Columns
{
    public class AvgColumnTest
    {
        [Fact]
        public void ConstructorTestWithNullStringArgument()
        {
            Action action = () => new AvgFunction((string)null);

            action.ShouldThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should()
                    .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ConstructorTestWithEmptyStringArgument()
        {
            Action action = () => new AvgFunction(string.Empty);

            action.ShouldThrowExactly<ArgumentOutOfRangeException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();

        }

        [Fact]
        public void ConstructorTestWithWhitespaceStringArgument()
        {
            Action action = () => new AvgFunction("   ");

            action.ShouldThrowExactly<ArgumentOutOfRangeException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ConstructorTestWithNullColumnArgument()
        {
            Action action = () => new AvgFunction((IColumn) null);

            action.ShouldThrowExactly<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ConstructorTestColumnArgument()
        {
            Assert.Equal(AggregateType.Average, new AvgFunction("age").Type);
        }

        public static IEnumerable<object[]> AsTestCases
        {
            get
            {
                yield return new object[]
                {
                    new AvgFunction("age".Field()),
                    null,
                };

                yield return new object[]
                {
                    new AvgFunction("age".Field()).As(string.Empty),
                    string.Empty,
                };
            }
        }

        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(AvgFunction column, string expectedAlias)
            => column.Alias.Should().Be(expectedAlias);
    }
}