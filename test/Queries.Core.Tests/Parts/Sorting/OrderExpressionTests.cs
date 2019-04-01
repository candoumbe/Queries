using FluentAssertions;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Sorting;
using System;
using System.Collections.Generic;
using Xunit;
using static Queries.Core.Parts.Sorting.OrderDirection;

namespace Queries.Core.Tests.Parts.Sorting
{
    /// <summary>
    /// Unit tests for <see cref="OrderExpression"/>
    /// </summary>
    public class OrderExpressionTests
    {
        [Fact]
        public void CtorThrowArgumentNullExpressionWhenStringParameterIsNull()
        {
            // Act
            Action action = () => new OrderExpression((string)null);

            // Assert
            action.Should().Throw<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CtorThrowArgumentNullExpressionWhenColumnParameterIsNull()
        {
            // Act
            Action action = () => new OrderExpression((ColumnBase)null);

            // Assert
            action.Should().Throw<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[]
                {
                    new OrderExpression("Name"),
                    null,
                    false,
                    "The second element is null"
                };

                yield return new object[]
                {
                    new OrderExpression("Name"),
                    new OrderExpression("Name"),
                    true,
                    "Distinct instances with same column and direction"
                };

                yield return new object[]
                {
                    new OrderExpression("Name", Descending),
                    new OrderExpression("Name"),
                    false,
                    "Distinct instances with same column but different directions"
                };

                {
                    OrderExpression expression = new OrderExpression("Name");
                    yield return new object[]
                    {
                        expression,
                        expression,
                        true,
                        "Comparing to itself"
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(EqualsCases))]
        public void EqualsTests(IOrder first, object second, bool expected, string reason)
        {
            // Act
            bool actual = first.Equals(second);

            // Assert
            actual.Should()
                .Be(expected, reason);
        }
    }
}
