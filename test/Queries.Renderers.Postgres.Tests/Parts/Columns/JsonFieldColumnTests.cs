using FluentAssertions;
using Queries.Core.Parts.Columns;
using Queries.Renderers.Postgres.Parts.Columns;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Queries.Renderers.Postgres.Tests.Parts.Columns
{
    [UnitTest]
    [Feature(nameof(Postgres))]
    public class JsonFieldColumnTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public JsonFieldColumnTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "path")]
        [InlineData("data", null)]
        public void Ctor_Should_Throws_ArgumentNullException(string column, string path)
        {
            // Act
            Action ctorWithNullArgument = () => new JsonFieldColumn(column?.Field(), path);

            // Assert
            ctorWithNullArgument.Should()
                .ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("data", "path")]
        public void CtorFeedsProperty(string columnName, string path)
        {
            // Arrange
            FieldColumn f = new FieldColumn(columnName);

            // Act
            JsonFieldColumn column = new JsonFieldColumn(f, path);

            // Assert
            column.Column.Should()
                .BeSameAs(f);
            column.Path.Should()
                .Be(path);
        }

        [Fact]
        public void Clone_Performs_A_Deep_Clone()
        {
            // Arrange
            FieldColumn f = new FieldColumn("prop1");
            JsonFieldColumn original = new JsonFieldColumn(f, "path");

            // Act
            IColumn clone = original.Clone();

            // Assert
            clone.Should()
                .BeOfType<JsonFieldColumn>().And
                .NotBeSameAs(original).And
                .Be(original);
        }

        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[] { new JsonFieldColumn("prop".Field(), "path"), null, false, "The second parameter is null" };
                yield return new object[] { new JsonFieldColumn("prop".Field(), "path"), "prop".Field(), false, $"The second parameter is not an instance of {nameof(JsonFieldColumn)} type" };
                yield return new object[] { new JsonFieldColumn("prop".Field(), "path"), new JsonFieldColumn("prop".Field(), "path"), true, $"Two different instances of same type with equivalent value as parameter" };
            }
        }

        [Theory]
        [MemberData(nameof(EqualsCases))]
        public void ImplementsEquatable_Properly(JsonFieldColumn first, object second, bool expectedValue, string reason)
        {
            _outputHelper.WriteLine($"First element : {first}");
            _outputHelper.WriteLine($"second second : {second}");

            // Act
            bool actual = first.Equals(second);

            // Assert
            actual.Should()
                .Be(expectedValue, reason);
        }
    }
}
