using FluentAssertions;
using Queries.Core.Attributes;
using Queries.Core.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        [Fact]
        public void HasDataManipulationLanguageAttribute()
        {
            // Arrange
            TypeInfo typeInfo = typeof(DeleteQuery).GetTypeInfo();

            // Act
            DataManipulationLanguageAttribute attr = typeInfo.GetCustomAttribute<DataManipulationLanguageAttribute>();

            // Arrange
            attr.Should()
                .NotBeNull($"{nameof(DeleteQuery)} must be marked with {nameof(DataManipulationLanguageAttribute)}");
        }

    }
}
