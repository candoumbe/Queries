using FluentAssertions;
using Queries.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;

namespace Queries.Core.Tests.Attributes
{
    public class FunctionAttributeTests
    {

        [Fact]
        public void IsValid()
        {
            // Arrange
            TypeInfo typeInfo = typeof(FunctionAttribute)
                .GetTypeInfo();

            // Act
            AttributeUsageAttribute attr = typeInfo.GetCustomAttribute<AttributeUsageAttribute>();

            // Assert
            attr.AllowMultiple.Should()
                .BeFalse("multiple usage of this attribute on the same element is not allowed");
            attr.Inherited.Should()
                .BeTrue();

            attr.ValidOn.Should().Be(AttributeTargets.Class);

        }
    }
}
