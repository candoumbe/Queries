using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AwesomeAssertions;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Xunit;
using Xunit.Categories;

namespace Queries.Core.Tests.Extensions;

[UnitTest]
[Feature(nameof(FieldColumn))]
[Feature("Extensions")]
public class FieldColumnExtensionsTests
{
    public static TheoryData<FieldColumn, string, UpdateFieldValue> EqualToExtensionCases
        => new()
        {
            {
                new FieldColumn("firstname"),
                "Bruce",
                new UpdateFieldValue("firstname".Field(), "Bruce".Literal())
            },
            {
                new FieldColumn("firstname"),
                null,
                new UpdateFieldValue("firstname".Field(), null)
            }
        };

    [Theory]
    [MemberData(nameof(EqualToExtensionCases))]
    public void EqualToExtension(FieldColumn fc, ColumnBase value, UpdateFieldValue expected)
    {
        // Act
        UpdateFieldValue actual  = fc.UpdateValueTo(value);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void EqualToExtensionThrowsArgumentNullExceptionWhenDestinationIsNull()
    {
        // Act
        Action action = () => ((FieldColumn)null).UpdateValueTo("Bruce");

        // Assert
        action.Should().Throw<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void IsNullExtension()
    {
        // Arrange
        FieldColumn fc = new("firstname");

        // Act
        WhereClause clause = fc.IsNull();

        // Assert
        clause.Column.Should().Be(fc);
        clause.Operator.Should().Be(ClauseOperator.IsNull);
        clause.Constraint.Should().BeNull();
    }

    [Fact]
    public void IsNullExtensionThrowsArgumentNullExceptionWhenDestinationIsNull()
    {
        // Act
        Action action = () => ((FieldColumn)null).IsNull();

        // Assert
        action.Should().Throw<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void IsNotNullExtension()
    {
        // Arrange
        FieldColumn fc = new("firstname");

        // Act
        WhereClause clause = fc.IsNotNull();

        // Assert
        clause.Column.Should().Be(fc);
        clause.Operator.Should().Be(ClauseOperator.IsNotNull);
        clause.Constraint.Should().BeNull();
    }

    [Fact]
    public void IsNotNullExtensionThrowsArgumentNullExceptionWhenDestinationIsNull()
    {
        // Act
        Action action = () => ( (FieldColumn )null).IsNotNull();

        // Assert
        action.Should().Throw<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void LessThanExtension()
    {
        // Arrange
        FieldColumn fc = new("age");

        // Act
        WhereClause clause = fc.LessThan(18);

        // Assert
        clause.Column.Should().Be(fc);
        clause.Operator.Should().Be(ClauseOperator.LessThan);
        clause.Constraint.Should()
            .BeOfType<NumericColumn>().Which
            .Value.Should()
            .Be(18);
    }

    [Fact]
    public void LessThanExtensionThrowsArgumentNullExceptionWhenDestinationIsNull()
    {
        // Act
        Action action = () => ((FieldColumn)null).LessThan(18);

        // Assert
        action.Should().Throw<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GreaterThanExtension()
    {
        // Arrange
        FieldColumn fc = new("age");

        // Act
        WhereClause clause = fc.GreaterThan(18);

        // Assert
        clause.Column.Should().Be(fc);
        clause.Operator.Should().Be(ClauseOperator.GreaterThan);
        clause.Constraint.Should()
            .BeAssignableTo<NumericColumn>().Which
            .Value.Should()
            .Be(18);
    }

    [Fact]
    public void GreaterThanExtensionThrowsArgumentNullExceptionWhenDestinationIsNull()
    {
        // Act
        Action action = () => ((FieldColumn)null).GreaterThan(18);

        // Assert
        action.Should().Throw<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GreaterThanOrEqualExtension()
    {
        // Arrange
        FieldColumn fc = new("age");

        // Act
        WhereClause clause = fc.GreaterThanOrEqualTo(18);

        // Assert
        clause.Column.Should().Be(fc);
        clause.Operator.Should().Be(ClauseOperator.GreaterThanOrEqualTo);
        clause.Constraint.Should()
            .BeOfType<NumericColumn>().Which
            .Value.Should()
            .Be(18);
    }

    [Fact]
    public void GreaterThanOrEqualToExtensionThrowsArgumentNullExceptionWhenDestinationIsNull()
    {
        // Act
        Action action = () => ((FieldColumn)null).GreaterThanOrEqualTo(18);

        // Assert
        action.Should().Throw<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void LessThanOrEqualExtension()
    {
        // Arrange
        FieldColumn fc = new("age");

        // Act
        WhereClause clause = fc.LessThanOrEqualTo(18);

        // Assert
        clause.Column.Should().Be(fc);
        clause.Operator.Should().Be(ClauseOperator.LessThanOrEqualTo);
        clause.Constraint.Should()
            .BeOfType<NumericColumn>().Which
            .Value.Should()
            .Be(18);
    }

    [Fact]
    public void LessThanOrEqualToExtensionThrowsArgumentNullExceptionWhenDestinationIsNull()
    {
        // Act
        Action action = () => ((FieldColumn)null).LessThanOrEqualTo(18);

        // Assert
        action.Should().Throw<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }
}