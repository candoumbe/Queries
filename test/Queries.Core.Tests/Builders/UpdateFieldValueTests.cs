using FluentAssertions;
using Queries.Core.Attributes;
using Queries.Core.Builders;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Builders
{
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
            action.ShouldThrow<ArgumentNullException>("name of the table to delete cannot be null").Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }



        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[] { "firstname".Field().EqualTo("Bruce"), null, false, "comparing with a null instance" };
                yield return new object[] { "firstname".Field().EqualTo("Bruce"), "firstname".Field().EqualTo("Bruce"), true, "comparing two instances with same fieldname" };
                yield return new object[] { "firstname".Field().EqualTo("Bruce"), "Firstname".Field().EqualTo("Bruce"), false, "comparing two instances with same fieldname but different casing" };
                yield return new object[] { "firstname".Field().EqualTo("Bruce"), Select(1.Literal()), false, "comparing two different types of query" };
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
}
