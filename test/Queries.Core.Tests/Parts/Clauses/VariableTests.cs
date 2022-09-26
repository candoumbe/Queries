using FluentAssertions;
using Queries.Core.Parts.Clauses;
using System;
using Xunit;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Clauses;

[UnitTest]
public class VariableTests
{
    [Theory]
    [InlineData(VariableType.Boolean)]
    [InlineData(VariableType.Date)]
    [InlineData(VariableType.Numeric)]
    [InlineData(VariableType.String)]
    public void CtorDoesNotThrowsArgumentNullExceptionIfValueIsNull(VariableType variableType)
    {
        // Act
        Action action = () => new Variable("p", variableType, null);

        // Assert
        action.Should().NotThrow<ArgumentNullException>($"{nameof(Variable)}.{nameof(Variable.Value)} can be null");
    }

    [Theory]
    [InlineData(null, VariableType.Boolean)]
    [InlineData(null, VariableType.Date)]
    [InlineData(null, VariableType.Numeric)]
    [InlineData(null, VariableType.String)]
    public void CtorThrowsArgumentNullExceptionIfParameterNameIsNull(string parameterName, VariableType constraintType)
    {
        // Act
        Action action = () => new Variable(parameterName, constraintType, 3);

        // Assert
        action.Should().Throw<ArgumentNullException>($"{nameof(Variable)}.{nameof(Variable.Name)} cannot be null").Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData("", VariableType.Boolean)]
    [InlineData("  ", VariableType.Boolean)]
    [InlineData("", VariableType.Date)]
    [InlineData("  ", VariableType.Date)]
    [InlineData("", VariableType.Numeric)]
    [InlineData("  ", VariableType.Numeric)]
    [InlineData("", VariableType.String)]
    [InlineData("  ", VariableType.String)]
    public void CtorThrowsArgumentOutOfRangeExceptionIfParameterNameIsEmptyOrWhiteSpace(string parameterName, VariableType constraintType)
    {
        // Act
        Action action = () => new Variable(parameterName, constraintType, 3);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>($"{nameof(Variable)}.{nameof(Variable.Name)} cannot be null").Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData("Nickname", VariableType.Boolean, "nickname")]
    [InlineData("FirstName", VariableType.Boolean, "firstName")]
    public void CtorMakeParameterNameCamelCase(string parameterName, VariableType variableType, string expectedParameterName)
    {
        // Act
        Variable v = new Variable(parameterName, variableType, "a value");

        // Assert
        v.Name.Should().Be(expectedParameterName);
    }
}
