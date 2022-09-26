using FluentAssertions;
using Queries.Core.Parts.Clauses;
using System.Collections.Generic;
using FluentAssertions.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Queries.Core.Tests;

public class CompiledQueryTests
{
    private readonly ITestOutputHelper _outputHelper;

    public CompiledQueryTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

    public static IEnumerable<object[]> EqualsCases
    {
        get
        {
            yield return new object[]
            {
                new CompiledQuery("A string", null),
                null,
                false,
                "the current instance is compared to 'null'"
            };

            yield return new object[]
            {
                new CompiledQuery("a statement", null),
                new CompiledQuery("a statement", null),
                true,
                "the current instance is compared to another instance with same statement and variables"
            };

            yield return new object[]
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
            };

            yield return new object[]
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
            };
        }
    }

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void TestEquals(CompiledQuery query, object other, bool expected, string reason)
    {
        _outputHelper.WriteLine($"{nameof(query)} : {query}");
        _outputHelper.WriteLine($"{nameof(other)} : {other}");

        // Act
        bool actual = query.Equals(other);

        // Assert
        actual.Should()
            .Be(expected, reason);
    }
}
