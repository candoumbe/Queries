﻿using FluentAssertions;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Queries.Core.Parts.Columns.Tests
{
    public class FieldColumnExtensionsTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public FieldColumnExtensionsTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }


        public void Dispose() => _outputHelper = null;


        public static IEnumerable<object[]> EqualToExtensionCases
        {
            get
            {
                yield return new object[]
                {
                    new FieldColumn("firstname"),
                    "Bruce",
                    ((Expression<Func<UpdateFieldValue, bool>>)(x => 
                        x.Source != null 
                        && x.Source is LiteralColumn 
                        && ((LiteralColumn)x.Source).Value is string 
                        && "Bruce".Equals(((LiteralColumn)x.Source).Value)
                        && x.Destination != null 
                        && "firstname".Equals(x.Destination.Name)    
                    ))
                };

                yield return new object[]
                {
                    new FieldColumn("firstname"),
                    null,
                    ((Expression<Func<UpdateFieldValue, bool>>)(x =>
                        x.Source  == null
                        && x.Destination != null
                        && "firstname".Equals(x.Destination.Name)
                    ))
                };
            }
        }

        [Theory]
        [MemberData(nameof(EqualToExtensionCases))]
        public void EqualToExtension(FieldColumn fc, ColumnBase value, Expression<Func<UpdateFieldValue, bool>> expectation)
        {
            // Act
            UpdateFieldValue ufv  = fc.EqualTo(value);

            // Assert
            ufv.Should().Match(expectation);
        }

        [Fact]
        public void EqualToExtensionThrowsArgumentNullExceptionWhenDestinationIsNull()
        {
            // Arrange
            FieldColumn fieldColumn = null;

            // Act
            Action action = () => fieldColumn.EqualTo("Bruce");

            // Assert
            action.ShouldThrow<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }


        [Fact]
        public void IsNullExtension()
        {
            // Arrange
            FieldColumn fc = new FieldColumn("firstname");

            // Act
            WhereClause clause = fc.IsNull();

            // Assert
            clause.Column.ShouldBeEquivalentTo(fc);
            clause.Operator.Should().Be(ClauseOperator.IsNull);
            clause.Constraint.Should().BeNull();
        }

        [Fact]
        public void IsNullExtensionThrowsArgumentNullExceptionWhenDestinationIsNull()
        {
            // Arrange
            FieldColumn fieldColumn = null;

            // Act
            Action action = () => fieldColumn.IsNull();

            // Assert
            action.ShouldThrow<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void IsNotNullExtension()
        {
            // Arrange
            FieldColumn fc = new FieldColumn("firstname");

            // Act
            WhereClause clause = fc.IsNotNull();

            // Assert
            clause.Column.ShouldBeEquivalentTo(fc);
            clause.Operator.Should().Be(ClauseOperator.IsNotNull);
            clause.Constraint.Should().BeNull();
        }

        [Fact]
        public void IsNotNullExtensionThrowsArgumentNullExceptionWhenDestinationIsNull()
        {
            // Arrange
            FieldColumn fieldColumn = null;

            // Act
            Action action = () => fieldColumn.IsNotNull();

            // Assert
            action.ShouldThrow<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void LessThanExtension()
        {
            // Arrange
            FieldColumn fc = new FieldColumn("age");

            // Act
            WhereClause clause = fc.LessThan(18);

            // Assert
            clause.Column.ShouldBeEquivalentTo(fc);
            clause.Operator.Should().Be(ClauseOperator.LessThan);
            clause.Constraint.Should()
                .BeOfType<NumericColumn>().Which
                .Value.Should()
                .Be(18);
        }

        [Fact]
        public void LessThanExtensionThrowsArgumentNullExceptionWhenDestinationIsNull()
        {
            // Arrange
            FieldColumn fieldColumn = null;

            // Act
            Action action = () => fieldColumn.LessThan(18);

            // Assert
            action.ShouldThrow<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GreaterThanExtension()
        {
            // Arrange
            FieldColumn fc = new FieldColumn("age");

            // Act
            WhereClause clause = fc.GreaterThan(18);

            // Assert
            clause.Column.ShouldBeEquivalentTo(fc);
            clause.Operator.Should().Be(ClauseOperator.GreaterThan);
            clause.Constraint.Should()
                .BeAssignableTo<NumericColumn>().Which
                .Value.Should()
                .Be(18);
        }

        [Fact]
        public void GreaterThanExtensionThrowsArgumentNullExceptionWhenDestinationIsNull()
        {
            // Arrange
            FieldColumn fieldColumn = null;

            // Act
            Action action = () => fieldColumn.GreaterThan(18);

            // Assert
            action.ShouldThrow<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GreaterThanOrEqualExtension()
        {
            // Arrange
            FieldColumn fc = new FieldColumn("age");

            // Act
            WhereClause clause = fc.GreaterThanOrEqualTo(18);

            // Assert
            clause.Column.ShouldBeEquivalentTo(fc);
            clause.Operator.Should().Be(ClauseOperator.GreaterThanOrEqualTo);
            clause.Constraint.Should()
                .BeOfType<NumericColumn>().Which
                .Value.Should()
                .Be(18);
        }

        [Fact]
        public void GreaterThanOrEqualToExtensionThrowsArgumentNullExceptionWhenDestinationIsNull()
        {
            // Arrange
            FieldColumn fieldColumn = null;

            // Act
            Action action = () => fieldColumn.GreaterThanOrEqualTo(18);

            // Assert
            action.ShouldThrow<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void LessThanOrEqualExtension()
        {
            // Arrange
            FieldColumn fc = new FieldColumn("age");

            // Act
            WhereClause clause = fc.LessThanOrEqualTo(18);

            // Assert
            clause.Column.ShouldBeEquivalentTo(fc);
            clause.Operator.Should().Be(ClauseOperator.LessThanOrEqualTo);
            clause.Constraint.Should()
                .BeOfType<NumericColumn>().Which
                .Value.Should()
                .Be(18);
        }

        [Fact]
        public void LessThanOrEqualToExtensionThrowsArgumentNullExceptionWhenDestinationIsNull()
        {
            // Arrange
            FieldColumn fieldColumn = null;

            // Act
            Action action = () => fieldColumn.LessThanOrEqualTo(18);

            // Assert
            action.ShouldThrow<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }


    }
}
