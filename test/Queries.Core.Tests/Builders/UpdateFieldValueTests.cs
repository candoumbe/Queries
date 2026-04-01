using AwesomeAssertions;
using Queries.Core.Builders;
using Queries.Core.Parts.Columns;
using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Builders;

[UnitTest]
[Feature("Builder")]
public class UpdateFieldValueTests(ITestOutputHelper outputHelper) : IDisposable
{
    private ITestOutputHelper _outputHelper = outputHelper;

    public void Dispose() => _outputHelper = null;

    [Fact]
    public void CtorWithNullFieldColumnArgumentThrowsArgumentNullException()
    {
        // Act
        Action action = () => _ = new UpdateFieldValue(null, 10);

        // Assert
        action.Should().Throw<ArgumentNullException>("name of the table to delete cannot be null").Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }



    public static TheoryData<UpdateFieldValue, object, bool, string> EqualsCases
        => new()
        {
            { "firstname".Field().UpdateValueTo("Bruce"), null, false, "comparing with a null instance" },
            { "firstname".Field().UpdateValueTo("Bruce"), "firstname".Field().UpdateValueTo("Bruce"), true, "comparing two instances with same fieldname" },
            { "firstname".Field().UpdateValueTo("Bruce"), "Firstname".Field().UpdateValueTo("Bruce"), false, "comparing two instances with same fieldname but different casing" },
            { "firstname".Field().UpdateValueTo("Bruce"), Select(1.Literal()), false, "comparing two different types of query" }
        };


    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualTests(UpdateFieldValue first, object second, bool expectedResult, string reason)
    {
        _outputHelper.WriteLine($"{nameof(first)} : {first}");
        _outputHelper.WriteLine($"{nameof(second)} : {second}");

        // Act
        bool actualResult = first.Equals(second);

        // Assert
        actualResult.Should().Be(expectedResult, reason);
    }
}