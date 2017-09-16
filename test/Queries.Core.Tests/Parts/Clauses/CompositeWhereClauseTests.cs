using FluentAssertions;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using static Queries.Core.Parts.Clauses.ClauseLogic;

namespace Queries.Core.Tests.Parts
{
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
            clause.GetParameters().Should().BeEmpty();
        }

        public static IEnumerable<object[]> GetParametersCases
        {
            get
            {
                yield return new object[]
                {
                    new CompositeWhereClause(),
                    ((Expression<Func<IEnumerable<Variable>, bool>>)(parameters => !parameters.Any()))
                };

                {
                    WhereClause ageLessThanFifteenCriterion = "Age".Field().LessThan(15);
                    WhereClause ageGreaterThanTenCriterion = "Age".Field().GreaterThan(10);
                    yield return new object[]
                    {
                        new CompositeWhereClause
                        {
                            Logic = And,
                            Clauses = new IWhereClause[]
                            {
                               ageLessThanFifteenCriterion,
                               ageGreaterThanTenCriterion
                            }
                        },
                        ((Expression<Func<IEnumerable<Variable>, bool>>)(parameters =>
                            parameters.Count() == 2
                            && parameters.Any(x => x.Type == VariableType.Numeric && 15.Literal().Equals(x.Value))
                            && parameters.Any(x => x.Type == VariableType.Numeric && 10.Literal().Equals(x.Value))
                        ))
                    };
                }

                {
                    WhereClause firstnameEqualToBruceCriterion = "Firstname".Field().LessThan("Bruce");
                    WhereClause ageGreaterThanTenCriterion = "Age".Field().GreaterThan(10);

                    yield return new object[]
                    {
                        new CompositeWhereClause
                        {
                            Logic = And,
                            Clauses = new IWhereClause[]
                            {
                                firstnameEqualToBruceCriterion,
                                ageGreaterThanTenCriterion
                            }
                        },
                        ((Expression<Func<IEnumerable<Variable>, bool>>)(parameters =>
                            parameters.Count() == 2
                            && parameters.Any(x => "Bruce".Literal().Equals(x.Value) && x.Type == VariableType.String)
                            && parameters.Any(x => 10.Literal().Equals(x.Value) && x.Type == VariableType.Numeric)
                        ))
                    };
                }
            }
        }



        [Theory]
        [MemberData(nameof(GetParametersCases))]
        public void GetParameters(CompositeWhereClause compositeWhereClause, Expression<Func<IEnumerable<Variable>, bool>> parametersExpectation)
        {
            // Act
            IEnumerable<Variable> parameters = compositeWhereClause.GetParameters();

            // Assert
            parameters.Should().Match(parametersExpectation);
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
