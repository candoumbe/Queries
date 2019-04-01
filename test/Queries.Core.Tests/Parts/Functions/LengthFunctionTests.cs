using FluentAssertions;
using Queries.Core.Attributes;
using Queries.Core.Parts.Functions;
using System;
using System.Reflection;
using Xunit;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Functions
{
    [UnitTest]
    [Feature(nameof(LengthFunction))]
    [Feature("Functions")]
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
        public void HaveFunctionAttribute() => typeof(LengthFunction).Should()
                .BeDecoratedWithOrInherit<FunctionAttribute>($"{nameof(LengthFunction)} must be marked with {nameof(FunctionAttribute)}");
    }
}
