using FluentAssertions;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Queries.Core.Tests.Parts.Columns
{
    public class LiteralColumnTests
    {
        [Fact]
        public void CtorWithNullArgumentDoesNotThrowArgumentNullException()
        {
            // Arrange
            Action action = () => new LiteralColumn(null);

            // Assert
            action.ShouldNotThrow<ArgumentException>();
        }

        [Fact]
        public void CtorWithNoArgumentDoesNotThrowArgumentNullException()
        {
            // Arrange
            Action action = () => new LiteralColumn();

            // Assert
            action.ShouldNotThrow<ArgumentException>();
        }



        [Fact]
        public void CtorThrowsArgumentExceptionWhenArgumentIsNotAPrimitiveType()
        {
            // Arrange
            Action action = () => new LiteralColumn(new { prop1 = "prop" });

            // Assert
            ArgumentException exception = action.ShouldThrow<ArgumentException>("only bool/int/double/float/long/string are supported").Which;

            exception
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();

            exception.Message.Should().NotBeNullOrWhiteSpace();
        }

        public static IEnumerable<object[]> CtorWithPrimitives
        {
            get
            {
                yield return new object[] { 1, "int value is supported" };
                yield return new object[] { true, "boolean value is supported" };
                yield return new object[] { false, "boolean value is supported" };
                yield return new object[] { 91f, $"{91f.GetType()} is supported" };
                yield return new object[] { 91L, $"{91L.GetType()} is supported" };
                yield return new object[] { new DateTime(1990, 2, 26), $"{nameof(DateTime)} is supported" };
                yield return new object[] { new DateTimeOffset(1990, 2, 26, 14, 30, 0, TimeSpan.Zero), $"{nameof(DateTimeOffset)} is supported" };
            }
        }

        [Theory]
        [MemberData(nameof(CtorWithPrimitives))]
        public void CtorSetInternalValueProperly(object value, string because)
        {
            // Act
            LiteralColumn literalColumn = new LiteralColumn(value);

            // Assert
            literalColumn.Value.Should().Be(value, because);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("newAlias")]
        public void SettingAlias(string newAlias)
        {
            // Arrange
            LiteralColumn column = new LiteralColumn("column");
            
            // Act
            column = column.As(newAlias);


            // Assert
            column.Alias.Should().Be(newAlias);
}
    }
}
