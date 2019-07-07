using FluentAssertions;
using Queries.Core.Builders;
using Queries.Core.Parts.Columns;
using System;
using Xunit;
using Xunit.Categories;
using static Queries.Renderers.Postgres.Builders.Fluent.ReturnBuilder;

namespace Queries.Renderers.Postgres.Tests.Builders.Fluent
{
    [UnitTest]
    [Feature(nameof(Postgres))]
    public class ReturnBuilderTests
    {
        [Fact]
        public void ThrowsArgumentException_When_ColumnBase_IsNull()
        {
            // Arrange
            ColumnBase returnValue = null;

            // Act
            Action returnWithNullColumnBase = () => Return(returnValue);

            // Assert
            returnWithNullColumnBase.Should()
                .ThrowExactly<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ThrowsArgumentException_When_SelectQuery_IsNull()
        {
            // Arrange
            SelectQuery returnValue = null;

            // Act
            Action returnWithNullSelectQuery = () => Return(returnValue);

            // Assert
            returnWithNullSelectQuery.Should()
                .ThrowExactly<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }
    }
}
