using FluentAssertions;
using Queries.Core.Extensions;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Queries.Core.Tests.Parts.Clauses
{
    public class WhereClauseTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public WhereClauseTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public void Dispose() => _outputHelper = null;

        [Fact]
        public void CtorShouldThrowsArgumentNullExceptionWhenColumnIsNull()
        {
            // Act
            Action action = () => new WhereClause(null, default);

            // Assert
            action.ShouldThrow<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();

        }

        public static IEnumerable<object[]> ObjectShouldBeInCorrectStateAfterBeingBuiltCases
        {
            get
            {
                yield return new object[]
                {
                    new LiteralColumn(1), ClauseOperator.EqualTo, 1
                };

                yield return new object[]
                {
                    new NumericColumn(1), ClauseOperator.EqualTo, "a"
                };
            }
        }

        [Theory]
        [MemberData(nameof(ObjectShouldBeInCorrectStateAfterBeingBuiltCases))]
        public void ObjectShouldBeInCorrectStateAfterBeingBuilt(IColumn column, ClauseOperator @operator, ColumnBase constraint)
        {
            // Act
            WhereClause clause = new WhereClause(column, @operator, constraint);

            // Assert
            clause.Column.Should().Be(column);
            clause.Operator.Should().Be(@operator);
            clause.Constraint.Should().Be(constraint);
        }


        [Fact]
        public void CtorShouldIgnoreConstraintWhenUsingIsNullOperator()
        {
            // Act
            WhereClause clause = new WhereClause("firstname".Field(), ClauseOperator.IsNull, 1);

            // Assert
            clause.Constraint.Should().BeNull();
        }

        [Fact]
        public void CtorShouldIgnoreConstraintWhenUsingIsNotNullOperator()
        {
            // Act
            WhereClause clause = new WhereClause("firstname".Field(), ClauseOperator.IsNotNull, 1);

            // Assert
            clause.Constraint.Should().BeNull();
        }

    }
}
