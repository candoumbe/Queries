﻿using FluentAssertions;
using Queries.Core.Attributes;
using System;
using System.Reflection;
using Xunit;
using Xunit.Categories;

namespace Queries.Core.Tests.Attributes;

[UnitTest]
[Feature(nameof(DataControlLanguageAttribute))]
[Feature("Attributes")]
public class DataControlLanguageAttributeTests
{
    [Fact]
    public void IsValid()
    {
        // Arrange
        TypeInfo typeInfo = typeof(DataControlLanguageAttribute)
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
