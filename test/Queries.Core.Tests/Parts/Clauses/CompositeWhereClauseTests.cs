using System;
using AwesomeAssertions;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Xunit;
using Xunit.Categories;
using static Queries.Core.Parts.Clauses.ClauseLogic;

namespace Queries.Core.Tests.Parts.Clauses;

[UnitTest]
[Feature("Where")]
public class CompositeWhereClauseTests
{
    [Fact]
    public void DefaultCtor()
    {
        // Act
        CompositeWhereClause clause = new();

        // Assert
        clause.Clauses.Should().BeEmpty();
        clause.Logic.Should().Be(And);
    }

    public static TheoryData<CompositeWhereClause, object, bool, string> EqualsCases
    {
        get
        {
            TheoryData<CompositeWhereClause, object, bool, string> cases = new()
            {
                {
                    new CompositeWhereClause
                    {
                        Logic = And,
                        Clauses = new IWhereClause[]
                        {
                           "Age".Field().LessThan(15),
                           "Age".Field().GreaterThan(10)
                        }
                    },
                    null,
                    false,
                    $"comparing {nameof(CompositeWhereClause)} instance to null"
                },
                {
                    new CompositeWhereClause
                    {
                        Logic = And,
                        Clauses = new IWhereClause[]
                        {
                           "Age".Field().LessThan(15),
                           "Age".Field().GreaterThan(10)
                        }
                    },
                    new CompositeWhereClause
                    {
                        Logic = And,
                        Clauses = new IWhereClause[]
                        {
                           "Age".Field().LessThan(15),
                           "Age".Field().GreaterThan(10)
                        }
                    },
                    true,
                    $"comparing two {nameof(CompositeWhereClause)} instances that holds same data"
                },
                {
                    new CompositeWhereClause
                    {
                        Logic = And,
                        Clauses = new IWhereClause[]
                        {
                           "Age".Field().LessThan(15),
                           "Age".Field().GreaterThan(10)
                        }
                    },
                    new CompositeWhereClause
                    {
                        Logic = And,
                        Clauses = new IWhereClause[]
                        {
                           "Age".Field().GreaterThan(10),
                           "Age".Field().LessThan(15)
                        }
                    },
                    false,
                    $"comparing two {nameof(CompositeWhereClause)} instances that holds same data but not in same order"
                }
            };

            CompositeWhereClause clause = new()
            {
                Logic = And,
                Clauses = new IWhereClause[]
                {
                   "Age".Field().LessThan(15),
                   "Age".Field().GreaterThan(10)
                }
            };
            cases.Add(clause, clause, true, $"comparing {nameof(CompositeWhereClause)} instance to itself");

            return cases;
        }
    }

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualsTests(CompositeWhereClause compositeWhereClause, object other, bool expectedResult, string reason)
    {
        // Act
        bool actualResult = compositeWhereClause.Equals(other);

        // Assert
        actualResult.Should().Be(expectedResult, reason);
    }
}
