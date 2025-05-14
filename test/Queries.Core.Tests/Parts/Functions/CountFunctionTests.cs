using System;
using FluentAssertions;
using FsCheck.Xunit;
using Queries.Core.Attributes;
using Queries.Core.Parts.Functions;
using TestsHelpers;
using Xunit;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Functions;

[UnitTest]
[Feature(nameof(CountFunction))]
[Feature("Functions")]
public class CountFunctionTests
{
    [Fact]
    public void ConstructorTestWithNullArgument()
    {
        // Act
        Action action = () => _ = new CountFunction(null);

        // Assert
        action.Should().Throw<ArgumentNullException>($"{nameof(CountFunction)} constructor called with null argument").Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Property(Arbitrary = [typeof(QueryGenerators)])]
    public void SettingAliasTest(CountFunction column, string newAlias)
    {
        // Act
        column = column.As(newAlias);

        // Assert
        _ = newAlias switch
        {
            null => column.Alias.Should().BeEmpty(),
            string value when string.IsNullOrEmpty(value) => column.Alias.Should().BeEmpty(),
            _ => column.Alias.Should().Be(newAlias)
        };
    }

    [Fact]
    public void HasFunctionAttribute() => typeof(CountFunction).Should()
            .BeDecoratedWithOrInherit<FunctionAttribute>($"{nameof(CountFunction)} must be marked with {nameof(FunctionAttribute)}");
}