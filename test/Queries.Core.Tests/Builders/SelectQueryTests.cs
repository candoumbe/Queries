using FluentAssertions;
using Queries.Core.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Builders
{
    /// <summary>
    /// Unit tests for <see cref="SelectQuery"/>.
    /// </summary>
    public class SelectQueryTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public SelectQueryTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public void Dispose() => _outputHelper = null;

        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[] { Select("Firstname"), null, false, "comparing with a null instance" };
                yield return new object[] { Select("Firstname"), Select("Firstname"), true, "comparing two instances with same columns names and same columns count" };
                yield return new object[] { Select(1.Literal()), Select(1.Literal()), true, "comparing two instances with same columns" };
            }
        }


        [Theory]
        [MemberData(nameof(EqualsCases))]
        public void EqualTests(SelectQuery first, object second, bool expectedResult, string reason)
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
