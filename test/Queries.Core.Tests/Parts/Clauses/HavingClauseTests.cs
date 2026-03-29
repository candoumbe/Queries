using AwesomeAssertions;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;
using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using static Queries.Core.Parts.Clauses.ClauseOperator;
namespace Queries.Core.Tests.Parts.Clauses
{
    [UnitTest]
    [Feature("Having")]
    public class HavingClauseTests(ITestOutputHelper outputHelper) : IDisposable
    {
        private ITestOutputHelper _outputHelper = outputHelper;

        public void Dispose() => _outputHelper = null;

        [Fact]
        public void CtorShouldThrowsArgumentNullExceptionWhenColumnIsNull()
        {
            // Act
            Action action = () => _ = new HavingClause(null, default);

            // Assert
            action.Should().Throw<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        public static TheoryData<AggregateFunction, ClauseOperator, ColumnBase> ObjectShouldBeInCorrectStateAfterBeingBuiltCases
        {
            get
            {
                ClauseOperator[] operators =
                [
                    EqualTo,
                    NotEqualTo,
                    GreaterThan,
                    GreaterThanOrEqualTo,
                    ClauseOperator.LessThan,
                    LessThanOrEqualTo,
                    NotEqualTo
                ];

                TheoryData<AggregateFunction, ClauseOperator, ColumnBase> cases = new();
                foreach (ClauseOperator op in operators)
                {
                    cases.Add(new MaxFunction("age"), op, 18);
                }
                return cases;
            }
        }

        [Theory]
        [MemberData(nameof(ObjectShouldBeInCorrectStateAfterBeingBuiltCases))]
        public void ObjectShouldBeInCorrectStateAfterBeingBuilt(AggregateFunction column, ClauseOperator @operator, ColumnBase constraint)
        {
            // Act
            HavingClause clause = new(column, @operator, constraint);

            // Assert
            clause.Column.Should().Be(column);
            clause.Operator.Should().Be(@operator);
            clause.Constraint.Should().Be(constraint);
        }

        public static TheoryData<HavingClause> CloneCases
            => new()
            {
                { new HavingClause(new CountFunction("Firstname".Field()), EqualTo, "Bruce") },
                { new HavingClause(new MinFunction("Firstname".Field()), IsNull, "Bruce") },
                { new HavingClause(new MaxFunction(1.Literal()), GreaterThanOrEqualTo, 2) },
                { new HavingClause(new AvgFunction(1.Literal()), GreaterThanOrEqualTo, 2) }
            };

        [Theory]
        [MemberData(nameof(CloneCases))]
        public void CloneTest(HavingClause original)
        {
            _outputHelper.WriteLine($"{nameof(original)} : {original}");

            // Act
            IHavingClause copy = original.Clone();

            // Assert
            copy.Should()
                .BeOfType<HavingClause>().Which.Should()
                .NotBeSameAs(original).And
                .Be(original);
        }
    }
}