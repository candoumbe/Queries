using FluentAssertions;

using Queries.Core.Attributes;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;

using System;
using System.Collections.Generic;

using Xunit;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Functions;

[UnitTest]
[Feature(nameof(SubstringFunction))]
[Feature("Functions")]
public class SubstringFunctionTests
{
    public static IEnumerable<object[]> CtorThrowsArgumentOutOfRangeExceptionCases
    {
        get
        {
            IColumn column = "Firstname".Field();
            yield return new object[]{column, -1, null, $"start value is negative"};
            yield return new object[]{column, 1, -1, $"length value is negative"};
        }
    }

    [Fact]
    public void CtorThrowsArgumentNullException()
    {
        // Act
        Action action = () => new SubstringFunction(null, 1, 1);

        // Assert
        action.Should().Throw<ArgumentNullException>($"first parameter of {nameof(SubstringFunction)}'s constructor cannot be null")
            .Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(nameof(CtorThrowsArgumentOutOfRangeExceptionCases))]
    public void CtorThrowsArgumentOutOfRangeExceptionIfAnyParameterIsNull(IColumn column, int start, int? length, string reason)
    {
        // Act
        Action action = () => new SubstringFunction(column, start, length);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>(reason).Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void IsMarkedWithFunctionAttribute() => typeof(SubstringFunction).Should()
        .BeDecoratedWith<FunctionAttribute>($"{nameof(SubstringFunction)} must be marked with {nameof(FunctionAttribute)}");
}
