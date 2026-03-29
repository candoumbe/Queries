using AwesomeAssertions;

using Queries.Core.Parts;

using System;

using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts;

/// <summary>
/// Unit tests for <see cref="Table"/>
/// </summary>
[UnitTest]
[Feature(nameof(Table))]
public class TableTests(ITestOutputHelper outputHelper)
{
    [Fact]
    public void CtorThrowsArgumentNullExceptionWhenParameterIsNull()
    {
        // Act
        Action action = () => _ = new Table(null);

        // Assert
        action.Should().Throw<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    public static TheoryData<Table, object, bool, string> EqualsCases
    {
        get
        {
            TheoryData<Table, object, bool, string> cases = new()
            {
                { new Table("firstname"), null, false, "object is null" },
                { new Table("firstname"), new Table("firstname"), true, $"object is a {nameof(Table)} with exactly the same {nameof(Table.Name)} and {nameof(Table.Alias)}" }
            };

            Table column = new("firstname");
            cases.Add(column, column, true, "Equals with same instance");

            return cases;
        }
    }

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualTests(Table first, object second, bool expectedResult, string reason)
    {
        outputHelper.WriteLine($"First : {first}");
        outputHelper.WriteLine($"Second : {second}");

        // Act
        bool actualResult = first.Equals(second);

        // Assert
        actualResult.Should().Be(expectedResult, reason);
    }
}