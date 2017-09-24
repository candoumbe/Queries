using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;
using FluentAssertions;
namespace Queries.Core.Tests.Parts.Columns
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
        public void ConstructorTestColumnArgument() => new AvgFunction("age").Type.Should().Be(AggregateType.Average);

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


        public static IEnumerable<object[]> CloneCases
        {
            get
            {
                yield return new[] { new AvgFunction("Firstname")};
                yield return new[] { new AvgFunction("Firstname".Field()) };
            }
        }

        [Theory]
        [MemberData(nameof(CloneCases))]
        public void CloneTest(AvgFunction original)
        {
            // Act
            IColumn copie = original.Clone();

            // Assert
            copie.Should()
                .BeOfType<AvgFunction>().Which.Should()
                .NotBeSameAs(original).And
                .Be(original);
        }

    }
}