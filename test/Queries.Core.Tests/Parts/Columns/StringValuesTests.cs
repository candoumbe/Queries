using AwesomeAssertions;
using Queries.Core.Parts.Columns;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Columns;

[UnitTest]
public class StringValuesTests(ITestOutputHelper outputHelper)
{
    public static TheoryData<StringValues, object, bool, string> EqualsCases
        => new()
        {
            { new StringValues("one", "two", "three"), null, false, "comparing with a null instance" },
            { new StringValues("one", "two", "three"), new StringValues("one", "two", "three"), true, "comparing two instances with same values of same type" },
            { new StringValues("one", "two", "three"), new StringValues("one", "three", "two"), false, "comparing two instances with of same values but in different order." }
        };

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualTests(StringValues first, object second, bool expectedResult, string reason)
    {
        outputHelper.WriteLine($"{nameof(first)} : {first}");
        outputHelper.WriteLine($"{nameof(second)} : {second}");

        // Act
        bool actualResult = first.Equals(second);

        // Assert
        actualResult.Should().Be(expectedResult, reason);
    }

    public static TheoryData<StringValues> CloneCases
        => new()
        {
            { new StringValues("Alice", "Bob", "Charles") }
        };

    [Theory]
    [MemberData(nameof(CloneCases))]
    public void CloneTest(StringValues original)
    {
        outputHelper.WriteLine($"{nameof(original)} : {original}");

        // Act
        IColumn copie = original.Clone();

        // Assert
        copie.Should()
            .BeOfType<StringValues>().Which.Should()
            .NotBeSameAs(original).And
            .BeEquivalentTo(original);
    }
}