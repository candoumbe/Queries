using AwesomeAssertions;
using Queries.Core.Parts.Clauses;
using AwesomeAssertions.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Queries.Core.Tests;

public class CompiledQueryTests(ITestOutputHelper outputHelper)
{
    public static TheoryData<CompiledQuery, object, bool, string> EqualsCases
        => new()
        {
            {
                new CompiledQuery("A string", null),
                null,
                false,
                "the current instance is compared to 'null'"
            },
            {
                new CompiledQuery("a statement", null),
                new CompiledQuery("a statement", null),
                true,
                "the current instance is compared to another instance with same statement and variables"
            },
            {
                new CompiledQuery("a statement", new []{
                    new Variable("p0", VariableType.String, "Cape"),
                    new Variable("p1", VariableType.String, "No strength"),
                }),
                new CompiledQuery("a statement", new []{
                    new Variable("p1", VariableType.String, "No strength"),
                    new Variable("p0", VariableType.String, "Cape"),
                }),
                true,
                "the current instance is compared to another instance with same statement and variables are not in the same order"
            },
            {
                new CompiledQuery("a statement", new []{
                    new Variable("p0", VariableType.String, "Cape"),
                    new Variable("p1", VariableType.Date, 10.April(2010)),
                }),
                new CompiledQuery("a statement", new []{
                    new Variable("p1", VariableType.Date, 10.April(2010)),
                    new Variable("p0", VariableType.String, "Cape"),
                }),
                true,
                "the current instance is compared to another instance with same statement and variables are not in the same order"
            }
        };

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void TestEquals(CompiledQuery query, object other, bool expected, string reason)
    {
        outputHelper.WriteLine($"{nameof(query)} : {query}");
        outputHelper.WriteLine($"{nameof(other)} : {other}");

        // Act
        bool actual = query.Equals(other);

        // Assert
        actual.Should()
            .Be(expected, reason);
    }
}