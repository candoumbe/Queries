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
    public class DeleteQueryTests
    {
        [Fact]
        public void CtorWithNullArgumentThrowsArgumentNullException()
        {
            // Act
            Action action = () => new DeleteQuery(null);

            // Assert
            action.ShouldThrow<ArgumentNullException>("name of the table to delete cannot be null").Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        
    }
}
