using FluentAssertions;
using Queries.Core.Attributes;
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
            action.Should().Throw<ArgumentNullException>().Which
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
            FunctionAttribute attr = lengthFunctionType.GetCustomAttribute<FunctionAttribute>();

            // Assert
            attr.Should()
                .NotBeNull($"{nameof(LengthFunction)} must be marked with {nameof(FunctionAttribute)}");
        }
    }
}
