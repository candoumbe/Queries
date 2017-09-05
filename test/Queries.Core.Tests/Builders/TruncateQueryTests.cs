using FluentAssertions;
using Queries.Core.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Builders
{
    public class TruncateQueryTests
    {
        [Fact]
        public void CtorWithNullArgumentThrowsArgumentNullException()
        {
            // Act
            Action action = () => new TruncateQuery(null);

            // Assert
            action.ShouldThrow<ArgumentNullException>("name of the table to delete cannot be null").Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void CtorWithEmptyOrWhiteSpaceArgumentThrowsArgumentOutOfRangeException(string tableName)
        {
            // Act
            Action action = () => new TruncateQuery(tableName);

            // Assert
            action.ShouldThrow<ArgumentOutOfRangeException>("name of the table to delete cannot be null").Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }


    }
}
