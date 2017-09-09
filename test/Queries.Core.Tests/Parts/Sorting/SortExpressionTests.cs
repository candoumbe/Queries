using FluentAssertions;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Sorting;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Queries.Core.Tests.Parts.Sorting
{
    /// <summary>
    /// Unit tests for <see cref="SortExpression"/>
    /// </summary>
    public class SortExpressionTests
    {

        [Fact] 
        public void CtorThrowArgumentNullExpressionWhenStringParameterIsNull()
        {
            // Act
            Action action = () => new SortExpression((string)null);

            // Assert
            action.ShouldThrow<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CtorThrowArgumentNullExpressionWhenColumnParameterIsNull()
        {
            // Act
            Action action = () => new SortExpression((ColumnBase)null);

            // Assert
            action.ShouldThrow<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }
    }
}
