using FluentAssertions;
using Queries.Core.Builders;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Builders;

[UnitTest]
[Feature("Builder")]
public class UpdateFieldValueTests : IDisposable
{
    private ITestOutputHelper _outputHelper;

    public UpdateFieldValueTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

    public void Dispose() => _outputHelper = null;

    [Fact]
    public void CtorWithNullieldColumnArgumentThrowsArgumentNullException()
    {
        // Act
        Action action = () => new UpdateFieldValue(null, 10);

        // Assert
        action.Should().Throw<ArgumentNullException>("name of the table to delete cannot be null").Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }



    public static IEnumerable<object[]> EqualsCases
    {
        get
        {
            yield return new object[] { "firstname".Field().UpdateValueTo("Bruce"), null, false, "comparing with a null instance" };
            yield return new object[] { "firstname".Field().UpdateValueTo("Bruce"), "firstname".Field().UpdateValueTo("Bruce"), true, "comparing two instances with same fieldname" };
            yield return new object[] { "firstname".Field().UpdateValueTo("Bruce"), "Firstname".Field().UpdateValueTo("Bruce"), false, "comparing two instances with same fieldname but different casing" };
            yield return new object[] { "firstname".Field().UpdateValueTo("Bruce"), Select(1.Literal()), false, "comparing two different types of query" };
        }
    }


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
