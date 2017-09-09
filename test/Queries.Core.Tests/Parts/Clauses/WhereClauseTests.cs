using FluentAssertions;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using static Queries.Core.Parts.Clauses.ClauseOperator;
using static Queries.Core.Builders.Fluent.QueryBuilder;

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
                    new LiteralColumn(1), EqualTo, 1
                };

                yield return new object[]
                {
                    new NumericColumn(1), EqualTo, "a"
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
            WhereClause clause = new WhereClause("firstname".Field(), IsNotNull, 1);

            // Assert
            clause.Constraint.Should().BeNull();
        }

        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[] { new WhereClause("firstname".Field(), EqualTo, "Bruce"), null, false, "comparing with a null instance" };
                yield return new object[] { new WhereClause("firstname".Field(), EqualTo, "Bruce"), new WhereClause("firstname".Field(), EqualTo, "Bruce"), true, "comparing two instances with same tableName" };
                yield return new object[] { new WhereClause("firstname".Field(), EqualTo, "Bruce"), new WhereClause("Firstname".Field(), EqualTo, "Bruce"), false, "comparing two instances with same criteria" };
                yield return new object[] { new WhereClause("firstname".Field(), EqualTo, "Bruce"), Select(1.Literal()), false, "comparing two different types of query" };
            }
        }


        [Theory]
        [MemberData(nameof(EqualsCases))]
        public void EqualTests(WhereClause first, object second, bool expectedResult, string reason)
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
