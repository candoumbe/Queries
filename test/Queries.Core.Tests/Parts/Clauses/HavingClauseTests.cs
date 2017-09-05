﻿using FluentAssertions;
using Queries.Core.Extensions;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using static Queries.Core.Parts.Clauses.ClauseOperator;
namespace Queries.Core.Tests.Parts.Clauses
{
    public class HavingClauseTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public HavingClauseTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public void Dispose() => _outputHelper = null;

        [Fact]
        public void CtorShouldThrowsArgumentNullExceptionWhenColumnIsNull()
        {
            // Act
            Action action = () => new HavingClause(null, default);

            // Assert
            action.ShouldThrow<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();

        }

        public static IEnumerable<object[]> ObjectShouldBeInCorrectStateAfterBeingBuiltCases
        {
            get
            {



                ClauseOperator[] @operators = new[]
                {
                    EqualTo,
                    NotEqualTo,
                    GreaterThan,
                    GreaterThanOrEqualTo,
                    LessThan,
                    LessThanOrEqualTo,
                    NotEqualTo
                };

                return operators
                    .Select(op => new object[] { new MaxFunction("age"), op, 18 });
            }
        }

        [Theory]
        [MemberData(nameof(ObjectShouldBeInCorrectStateAfterBeingBuiltCases))]
        public void ObjectShouldBeInCorrectStateAfterBeingBuilt(AggregateFunction column, ClauseOperator @operator, ColumnBase constraint)
        {
            // Act
            HavingClause clause = new HavingClause(column, @operator, constraint);

            // Assert
            clause.Column.Should().Be(column);
            clause.Operator.Should().Be(@operator);
            clause.Constraint.Should().Be(constraint);
        }





        

    }
}
