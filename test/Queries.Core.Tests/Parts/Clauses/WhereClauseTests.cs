using AwesomeAssertions;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using static Queries.Core.Parts.Clauses.ClauseOperator;

namespace Queries.Core.Tests.Parts.Clauses;

[UnitTest]
[Feature("Where")]
public class WhereClauseTests(ITestOutputHelper outputHelper)
{
    [Theory]
    [InlineData(EqualTo)]
    [InlineData(GreaterThan)]
    [InlineData(GreaterThanOrEqualTo)]
    [InlineData(In)]
    [InlineData(IsNotNull)]
    [InlineData(IsNull)]
    [InlineData(ClauseOperator.LessThan)]
    [InlineData(LessThanOrEqualTo)]
    [InlineData(Like)]
    [InlineData(NotEqualTo)]
    [InlineData(NotLike)]
    public void CtorShouldThrowsArgumentNullExceptionWhenColumnIsNull(ClauseOperator @operator)
    {
        // Act
        Action action = () => _ = new WhereClause(null, @operator);

        // Assert
        action.Should().Throw<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    public static TheoryData<ClauseOperator, IColumn, string> CtorThrowsArgumentNullExceptionCases
        => new()
        {
            {
                In,
                null,
                $"The column constraint cannot be null when using '{nameof(In)}' operator"
            }
        };

    [Theory]
    [MemberData(nameof(CtorThrowsArgumentNullExceptionCases))]
    public void CtorThrowsArgumentNullExceptionWhenValueIsNull(ClauseOperator @operator, IColumn value, string reason)
    {
        // Act
        Action action = () => _ = new WhereClause("Firstname".Field(), @operator, value);

        // Assert
        action.Should()
            .Throw<ArgumentNullException>(reason).Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    public static TheoryData<IColumn, ClauseOperator, ColumnBase, Expression<Func<WhereClause, bool>>> ObjectShouldBeInCorrectStateAfterBeingBuiltCases
        => new()
        {
            {
                1.Literal(),
                EqualTo,
                1,
                (Expression<Func<WhereClause, bool>>)(clause =>
                    1.Literal().Equals(clause.Column)
                    && EqualTo == clause.Operator
                    && 1.Literal().Equals(clause.Constraint)
                )
            },
            {
                new NumericColumn(1),
                EqualTo,
                "a",
                (Expression<Func<WhereClause, bool>>)(clause =>
                    1.Literal().Equals(clause.Column)
                    && EqualTo == clause.Operator
                    && "a".Literal().Equals(clause.Constraint)
                )
            },
            {
                "Firstname".Field(),
                ClauseOperator.LessThan,
                "Bruce",
                (Expression<Func<WhereClause, bool>>)(clause =>
                    "Firstname".Field().Equals(clause.Column)
                    && ClauseOperator.LessThan == clause.Operator
                    && "Bruce".Literal().Equals(clause.Constraint)
                )
            },
            {
                "Firstname".Field(),
                In,
                new StringValues("Bruce", "Lex", "Clark"),
                (Expression<Func<WhereClause, bool>>)(clause =>
                    "Firstname".Field().Equals(clause.Column)
                    && In == clause.Operator
                    && new StringValues("Bruce", "Lex", "Clark").Equals(clause.Constraint)
                )
            }
        };

    [Theory]
    [MemberData(nameof(ObjectShouldBeInCorrectStateAfterBeingBuiltCases))]
    public void ObjectShouldBeInCorrectStateAfterBeingBuilt(IColumn column, ClauseOperator @operator, ColumnBase constraint,
        Expression<Func<WhereClause, bool>> expectation)
    {
        // Act
        WhereClause clause = new(column, @operator, constraint);

        // Assert
        clause.Should().Match(expectation);
    }

    [Fact]
    public void CtorShouldIgnoreConstraintWhenUsingIsNullOperator()
    {
        // Act
        WhereClause clause = new("firstname".Field(), IsNull, 1);

        // Assert
        clause.Constraint.Should().BeNull();
    }

    [Fact]
    public void CtorShouldIgnoreConstraintWhenUsingIsNotNullOperator()
    {
        // Act
        WhereClause clause = new("firstname".Field(), IsNotNull, 1);

        // Assert
        clause.Constraint.Should().BeNull();
    }

    public static TheoryData<WhereClause, object, bool, string> EqualsCases
        => new()
        {
            {
                new WhereClause("firstname".Field(), EqualTo, "Bruce"),
                null,
                false,
                "comparing with a null instance"
            },
            { new WhereClause("firstname".Field(), EqualTo, "Bruce"), new WhereClause("firstname".Field(), EqualTo, "Bruce"), true, "comparing two instances with same columns and constrains" },
            { new WhereClause("firstname".Field(), EqualTo, "Bruce"), new WhereClause("Firstname".Field(), EqualTo, "Bruce"), false, "comparing two instances with same columns but different casing" },
            { new WhereClause("firstname".Field(), EqualTo, "Bruce"), Select(1.Literal()), false, "comparing two different types of query" },
            { new WhereClause("firstname".Field(), EqualTo, new Variable("p0", VariableType.String, "Bruce")), new WhereClause("firstname".Field(), EqualTo, new Variable("p0", VariableType.String, "Bruce")), true, "comparing two different types with same data" },
            { new WhereClause("firstname".Field(), EqualTo, new Variable("p0", VariableType.String, "Bruce")), new WhereClause("firstname".Field(), EqualTo, new Variable("p0", VariableType.String, "Bruce")), true, "comparing two different types with same data" },
            {
                new WhereClause("firstname".Field(), EqualTo, "Bruce"),
                "firstname".Field().EqualTo("Bruce"),
                true,
                "comparing a to a fluent instance with same columns and constrains"
            },
            {
                new WhereClause("XP".Field(), LessThanOrEqualTo, 10),
                new WhereClause("XP".Field(), LessThanOrEqualTo, 10L),
                true,
                "Comparing 2 where clauses with same field and long/int constraint"
            },
            {
                new WhereClause("XP".Field(), LessThanOrEqualTo, 6.4m),
                new WhereClause("XP".Field(), LessThanOrEqualTo, 6.4m),
                true,
                "Comparing 2 where clauses with same field and decimal constraint"
            },
            {
                new WhereClause("XP".Field(), LessThanOrEqualTo, 6m),
                new WhereClause("XP".Field(), LessThanOrEqualTo, 6),
                true,
                "Comparing 2 where clauses with same field and decimal/int constraints"
            }
        };

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualTests(WhereClause first, object second, bool expectedResult, string reason)
    {
        outputHelper.WriteLine($"{nameof(first)} : {first}");
        outputHelper.WriteLine($"{nameof(second)} : {second}");

        // Act
        bool actualResult = first.Equals(second);

        // Assert
        actualResult.Should().Be(expectedResult, reason);
    }

    public static TheoryData<WhereClause> CloneCases
        => new()
        {
            new WhereClause("Firstname".Field(), EqualTo, "Bruce"),
            new WhereClause("Firstname".Field(), IsNull),
            new WhereClause("Firstname".Field(), IsNotNull, "Bruce"),
            new WhereClause(1.Literal(), ClauseOperator.LessThan, 2),
            new WhereClause(1.Literal(), GreaterThan, 2),
            new WhereClause(1.Literal(), GreaterThanOrEqualTo, 2),
            new WhereClause("Height".Field(), GreaterThanOrEqualTo, 2.3m)
        };

    [Theory]
    [MemberData(nameof(CloneCases))]
    public void CloneTest(WhereClause original)
    {
        outputHelper.WriteLine($"{nameof(original)} : {original}");

        // Act
        IWhereClause copie = original.Clone();

        // Assert
        copie.Should()
            .BeOfType<WhereClause>().Which.Should()
            .NotBeSameAs(original).And
            .Be(original);
    }
}