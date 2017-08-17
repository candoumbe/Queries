using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;
using FluentAssertions;

namespace Queries.Core.Tests.Parts.Columns
{
    public class FieldColumnTest
    {
        [Fact]
        public void ConstructorTestWithNullArgument()
        {
            // Act
            Action action = () => new FieldColumn(null);

            // Arrange
            action.ShouldThrow<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ConstructorTestWithEmptyArgument()
        {
            // Act
            Action action = () => new FieldColumn(string.Empty);

            // Arrange
            action.ShouldThrow<ArgumentOutOfRangeException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ConstructorTestWithWhitespaceStringArgument()
        {
            // Act
            Action action = () => new FieldColumn("   ");

            // Arrange
            action.ShouldThrow<ArgumentOutOfRangeException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
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
        public void SettingAliasTest(CountFunction column, string expectedAlias)
            => column.Alias.Should().Be(expectedAlias);
    }
}