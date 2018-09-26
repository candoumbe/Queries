using FluentAssertions;
using Queries.Core.Attributes;
using System;
using System.Reflection;
using Xunit;

namespace Queries.Core.Tests.Attributes
{
    public class DataDefinitionLanguageAttributeTests
    {
        [Fact]
        public void IsValid()
        {
            // Arrange
            TypeInfo typeInfo = typeof(DataDefinitionLanguageAttribute)
                .GetTypeInfo();

            // Act
            AttributeUsageAttribute attr = typeInfo.GetCustomAttribute<AttributeUsageAttribute>();

            // Assert
            attr.AllowMultiple.Should()
                .BeFalse("multiple usage of this attribute on the same element is not allowed");
            attr.Inherited.Should()
                .BeTrue("the attribute must propagate to inherited classes ");

            attr.ValidOn.Should().Be(AttributeTargets.Class);
        }
    }
}
