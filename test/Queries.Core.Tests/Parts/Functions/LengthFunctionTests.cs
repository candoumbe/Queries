using FluentAssertions;
using Queries.Core.Parts.Functions;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Queries.Core.Tests.Parts.Functions
{
    public class LengthFunctionTests
    {

        [Fact]
        public void CtorThrowsArgumentNullExceptionIfAnyParameterIsNull()
        {
            // Act
            Action action = () => new LengthFunction(null);

            // Assert
            action.ShouldThrow<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();

        }

        [Fact]
        public void HaveFunctionAttribute()
        {
            // Arrange 
            TypeInfo lengthFunctionType = typeof(LengthFunction)
                .GetTypeInfo();

            // Act
            IEnumerable<CustomAttributeData> customAttributes = lengthFunctionType.CustomAttributes;

            // Assert
            customAttributes.Should()
                .ContainSingle(attr => attr.AttributeType.Equals(typeof(FunctionAttribute)));
        }
    }
}
