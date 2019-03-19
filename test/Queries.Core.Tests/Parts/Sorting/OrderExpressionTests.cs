using FluentAssertions;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Sorting;
using System;
using Xunit;

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
    }
}
