using FluentAssertions;
using Queries.Core.Builders;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
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


        public static IEnumerable<object[]> CtorThrowsArgumentOutOfRangeExceptionCases {
            get
            {
                yield return new object[] { Enumerable.Empty<IColumn>().ToArray(), $"empty array of {nameof(IColumn)}s" };
                yield return new object[] { Enumerable.Repeat<IColumn>(null, 5).ToArray(), $"array of 5 null {nameof(IColumn)}s" };
            }
        }

        [Theory]
        [MemberData(nameof(CtorThrowsArgumentOutOfRangeExceptionCases))]
        public void CtorThrowsArgumentOutOfRangeException(IEnumerable<IColumn> columns, string reason)
        {
            // Act
            Action action = () => new SelectQuery(columns.ToArray());

            // Assert
            action.ShouldThrow<ArgumentOutOfRangeException>("no columns set").Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }
    }
}
