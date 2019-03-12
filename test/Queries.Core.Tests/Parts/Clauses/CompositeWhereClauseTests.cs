using FluentAssertions;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Categories;
using static Queries.Core.Parts.Clauses.ClauseLogic;

namespace Queries.Core.Tests.Parts
{
    [UnitTest]
    [Feature("Where")]
    public class CompositeWhereClauseTests
    {
        [Fact]
        public void DefaultCtor()
        {
            // Act
            CompositeWhereClause clause = new CompositeWhereClause();

            // Assert
            clause.Clauses.Should().BeEmpty();
            clause.Logic.Should().Be(And);
        }
        
        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[]
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
                };

                yield return new object[]
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
                };

                yield return new object[]
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
                };
                {
                    CompositeWhereClause clause = new CompositeWhereClause
                    {
                        Logic = And,
                        Clauses = new IWhereClause[]
                            {
                               "Age".Field().LessThan(15),
                               "Age".Field().GreaterThan(10)
                            }
                    };
                    yield return new object[]
                    {
                        clause, clause,
                        true,
                        $"comparing {nameof(CompositeWhereClause)} instance to itself"
                    };
                }
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
}
