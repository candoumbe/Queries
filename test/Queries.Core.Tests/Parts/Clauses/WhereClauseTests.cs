using FluentAssertions;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using static Queries.Core.Parts.Clauses.ClauseOperator;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using System.Linq.Expressions;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Clauses
{
    [UnitTest]
    [Feature("Where")]
    public class WhereClauseTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public WhereClauseTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public void Dispose() => _outputHelper = null;

        [Theory]
        [InlineData(EqualTo)]
        [InlineData(GreaterThan)]
        [InlineData(GreaterThanOrEqualTo)]
        [InlineData(In)]
        [InlineData(IsNotNull)]
        [InlineData(IsNull)]
        [InlineData(LessThan)]
        [InlineData(LessThanOrEqualTo)]
        [InlineData(Like)]
        [InlineData(NotEqualTo)]
        [InlineData(NotLike)]
        public void CtorShouldThrowsArgumentNullExceptionWhenColumnIsNull(ClauseOperator @operator)
        {
            // Act
            Action action = () => new WhereClause(null, @operator);

            // Assert
            action.Should().Throw<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(In)]
        public void CtorThrowsArgumentNuullExceptionWhenValueIsNull(ClauseOperator @operator)
        {
            // Act
            Action action = () => new WhereClause("Firstname".Field(), @operator, null);

            // Assert
            action.Should().Throw<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        public static IEnumerable<object[]> ObjectShouldBeInCorrectStateAfterBeingBuiltCases
        {
            get
            {
                yield return new object[]
                {
                    new Literal(1), EqualTo, 1,
                    (Expression<Func<WhereClause, bool>>)(clause =>
                        1.Literal().Equals(clause.Column)
                        && EqualTo == clause.Operator
                        && 1.Literal().Equals(clause.Constraint)
                    )
                };

                yield return new object[]
                {
                    new NumericColumn(1), EqualTo, "a",
                    (Expression<Func<WhereClause, bool>>)(clause =>
                        1.Literal().Equals(clause.Column)
                        && EqualTo == clause.Operator
                        && "a".Literal().Equals(clause.Constraint)
                    )
                };

                yield return new object[]
                {
                    "Firstname".Field(), LessThan, $"{"Bruce"}",
                    (Expression<Func<WhereClause, bool>>)(clause =>
                        "Firstname".Field().Equals(clause.Column)
                        && LessThan == clause.Operator
                        && "Bruce".Literal().Equals(clause.Constraint)
                    )
                };

                yield return new object[]
                {
                    "Firstname".Field(), In, new StringValues("Bruce", "Lex", "Clark"),
                    (Expression<Func<WhereClause, bool>>)(clause =>
                        "Firstname".Field().Equals(clause.Column)
                        && In == clause.Operator
                        && new StringValues("Bruce", "Lex", "Clark").Equals(clause.Constraint)
                    )
                };
            }
        }

        [Theory]
        [MemberData(nameof(ObjectShouldBeInCorrectStateAfterBeingBuiltCases))]
        public void ObjectShouldBeInCorrectStateAfterBeingBuilt(IColumn column, ClauseOperator @operator, ColumnBase constraint,
            Expression<Func<WhereClause, bool>> expectation)
        {
            // Act
            WhereClause clause = new WhereClause(column, @operator, constraint);

            // Assert
            clause.Should().Match(expectation);
        }

        [Fact]
        public void CtorShouldIgnoreConstraintWhenUsingIsNullOperator()
        {
            // Act
            WhereClause clause = new WhereClause("firstname".Field(), IsNull, 1);

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

        public static IEnumerable<object[]> CloneCases
        {
            get
            {
                yield return new[] { new WhereClause("Firstname".Field(), EqualTo, "Bruce") };
                yield return new[] { new WhereClause("Firstname".Field(), IsNull, "Bruce") };
                yield return new[] { new WhereClause("Firstname".Field(), IsNotNull, "Bruce") };
                yield return new[] { new WhereClause(1.Literal(), LessThan, 2) };
                yield return new[] { new WhereClause(1.Literal(), GreaterThan, 2) };
                yield return new[] { new WhereClause(1.Literal(), GreaterThanOrEqualTo, 2) };
            }
        }

        [Theory]
        [MemberData(nameof(CloneCases))]
        public void CloneTest(WhereClause original)
        {
            _outputHelper.WriteLine($"{nameof(original)} : {original}");

            // Act
            IWhereClause copie = original.Clone();

            // Assert
            copie.Should()
                .BeOfType<WhereClause>().Which.Should()
                .NotBeSameAs(original).And
                .Be(original);
        }
    }
}
