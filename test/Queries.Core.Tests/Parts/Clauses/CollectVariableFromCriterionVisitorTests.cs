using FluentAssertions;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Xunit.Abstractions;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using static Queries.Core.Parts.Clauses.ClauseOperator;
using static Queries.Core.Parts.Clauses.ClauseLogic;
using Queries.Core.Parts.Columns;
using FluentAssertions.Extensions;

namespace Queries.Core.Tests.Parts.Clauses
{
    public class CollectVariableFromCriterionVisitorTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public CollectVariableFromCriterionVisitorTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public void Dispose() => _outputHelper = null;


        [Fact]
        public void CtorShouldBuildValidInstance()
        {
            // Act
            CollectVariableVisitor instance = new CollectVariableVisitor();

            // Assert
            instance.Should()
                .BeAssignableTo<IVisitor<SelectQuery>>().And
                .BeAssignableTo<IVisitor<InsertIntoQuery>>();
            instance.Variables.Should()
                .BeAssignableTo<IEnumerable<Variable>>().Which
                .Should().BeEmpty($"{nameof(CollectVariableVisitor)}.{nameof(CollectVariableVisitor.Variables)} should be empty by default");
        }

        public static IEnumerable<object[]> VisitSelectQueryCases
        {
            get
            {
                yield return new object[]
                {
                    Select("Fullname").From("SuperHero").Where("Nickname".Field(), Like, "Bat%"),
                    ((Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                        visitor.Variables.Count() == 1
                        && visitor.Variables.Any(x => x.Name == "p0" && "Bat%".Equals(x.Value) && x.Type == VariableType.String)
                    )),
                    ((Expression<Func<SelectQuery, bool>>)(query =>
                        query.Equals(Select("Fullname").From("SuperHero").Where("Nickname".Field(), Like, new Variable("p0", VariableType.String, "Bat%"))))
                    )
                };

                yield return new object[]
                {
                    Select("Fullname").From("SuperHero").Where("Nickname".Field(), In, new StringValues("Batman", "Superman")),
                    ((Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                        visitor.Variables.Count() == 2
                        && visitor.Variables.Any(x => x.Name == "p0" && "Batman".Equals(x.Value) && x.Type == VariableType.String)
                        && visitor.Variables.Any(x => x.Name == "p1" && "Superman".Equals(x.Value) && x.Type == VariableType.String)
                    )),
                    ((Expression<Func<SelectQuery, bool>>)(query =>
                        query.Equals(
                            Select("Fullname").From("SuperHero")
                            .Where("Nickname".Field(), In, new VariableValues(new Variable("p0", VariableType.String, "Batman"), new Variable("p1", VariableType.String, "Superman")))))
                    )
                };

                yield return new object[]
                {
                    Select("Fullname").From("SuperHero")
                        .Where(new CompositeWhereClause{
                            Logic = Or,
                            Clauses = new[]
                            {
                                new WhereClause("Nickname".Field(), Like, "Bat%"),
                                new WhereClause("CanFly".Field(), EqualTo, true)
                            }
                        }),
                    ((Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                        visitor.Variables.Count() == 2
                        && visitor.Variables.Any(x => x.Name == "p0" && "Bat%".Equals(x.Value) && x.Type == VariableType.String)
                        && visitor.Variables.Any(x => x.Name == "p1" && true.Equals(x.Value) && x.Type == VariableType.Boolean)
                    )),
                    ((Expression<Func<SelectQuery, bool>>)(query =>
                        query.Equals(
                            Select("Fullname")
                            .From("SuperHero")
                            .Where(new CompositeWhereClause
                                {
                                    Logic = Or,
                                    Clauses = new[]
                                    {
                                        new WhereClause("Nickname".Field(), Like, new Variable("p0", VariableType.String, "Bat%")),
                                        new WhereClause("CanFly".Field(), EqualTo, new Variable("p1", VariableType.Boolean, true))
                                    }
                                })
                        ))
                    )
                };

                yield return new object[]
                {
                    Select("Fullname").From("SuperHero")
                        .Where(new CompositeWhereClause
                            {
                                Logic = And,
                                Clauses = new IWhereClause[]
                                {
                                    new WhereClause("DateOfBirth".Field(), LessThan, 1.January(1990)),
                                    new CompositeWhereClause{
                                        Logic = Or,
                                        Clauses = new[]
                                        {
                                            new WhereClause("Nickname".Field(), Like, "Bat%"),
                                            new WhereClause("CanFly".Field(), EqualTo, true)
                                        }
                                    }
                                }
                            }),
                    ((Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                        visitor.Variables.Count() == 3
                        && visitor.Variables.Any(x => x.Name == "p0" && 1.January(1990).Equals(x.Value) && x.Type == VariableType.Date)
                        && visitor.Variables.Any(x => x.Name == "p1" && "Bat%".Equals(x.Value) && x.Type == VariableType.String)
                        && visitor.Variables.Any(x => x.Name == "p2" && true.Equals(x.Value) && x.Type == VariableType.Boolean)
                    )),
                    ((Expression<Func<SelectQuery, bool>>)(query =>
                        query.Equals(
                            Select("Fullname")
                            .From("SuperHero")
                            .Where(
                                new CompositeWhereClause
                            {
                                Logic = And,
                                Clauses = new IWhereClause[]
                                {
                                    new WhereClause("DateOfBirth".Field(), LessThan, new Variable("p0", VariableType.Date, 1.January(1990))),
                                    new CompositeWhereClause{
                                        Logic = Or,
                                        Clauses = new[]
                                        {
                                            new WhereClause("Nickname".Field(), Like, new Variable("p1", VariableType.String, "Bat%")),
                                            new WhereClause("CanFly".Field(), EqualTo, new Variable("p2", VariableType.Boolean, true))
                                        }
                                    }
                                }
                            })
                        ))
                    )
                };

                yield return new object[]
                {
                    Select("*")
                    .From(
                        Select("Fullname").From("People").Where("Firstname".Field(), Like, "B%")
                        .Union(
                        Select("Fullname").From("Superhero").Where("Nickname".Field(), Like, "B%"))
                    ),
                    ((Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                        visitor.Variables.Count() == 1
                        && visitor.Variables.Any(x => x.Name == "p0" && "B%".Equals(x.Value) && x.Type == VariableType.String)
                    )),
                    ((Expression<Func<SelectQuery, bool>>)(query =>
                        query.Equals(
                            Select("*")
                            .From(
                                Select("Fullname").From("People").Where("Firstname".Field(), Like, new Variable("p0", VariableType.String, "B%"))
                                .Union(
                                Select("Fullname").From("Superhero").Where("Nickname".Field(), Like, new Variable("p0", VariableType.String, "B%")))
                            ))
                    ))
                };
            }
        }

        /// <summary>
        /// Tests <see cref="CollectVariableVisitor.Visit(SelectQuery)"/>.
        /// </summary>
        /// <param name="selectQuery"><see cref="SelectQuery"/> to visit.</param>
        /// <param name="visitorExpectation">visitor' state after  visiting <paramref name="selectQuery"/></param>
        /// <param name="selectQueryExpectation"><paramref name="selectQuery"/>' state after being visited</param>
        [Theory]
        [MemberData(nameof(VisitSelectQueryCases))]
        public void VisitSelectQuery(SelectQuery selectQuery, Expression<Func<CollectVariableVisitor, bool>> visitorExpectation, Expression<Func<SelectQuery, bool>> selectQueryExpectation)
        {
            // Arrange
            CollectVariableVisitor visitor = new CollectVariableVisitor();
            _outputHelper.WriteLine($"{nameof(selectQuery)} : {selectQuery}");
            // Act
            visitor.Visit(selectQuery);

            // Assert
            visitor.Should()
                .Match(visitorExpectation);

            selectQuery.Should().Match(selectQueryExpectation);

        }


        public static IEnumerable<object[]> VisitInsertIntoQueryCases
        {
            get
            {
                yield return new object[]
                {
                    InsertInto("SuperHero")
                        .Values(
                            "Firstname".InsertValue("Clark".Literal()),
                            "Lastname".InsertValue("Kent".Literal()),
                            "Powers".InsertValue("Super strength".Literal())
                        ),
                    ((Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                        visitor.Variables.Count() == 3
                        && visitor.Variables.Any(x => x.Name == "p0" && "Clark".Equals(x.Value) && x.Type == VariableType.String)
                        && visitor.Variables.Any(x => x.Name == "p1" && "Kent".Equals(x.Value) && x.Type == VariableType.String)
                        && visitor.Variables.Any(x => x.Name == "p2" && "Super strength".Equals(x.Value) && x.Type == VariableType.String)
                    )),
                    ((Expression<Func<InsertIntoQuery, bool>>)(query =>
                        query.Equals(InsertInto("SuperHero").Values(
                            "Firstname".InsertValue(new Variable("p0", VariableType.String, "Clark")),
                            "Lastname".InsertValue(new Variable("p1", VariableType.String, "Kent")),
                            "Powers".InsertValue(new Variable("p2", VariableType.String, "Super strength"))
                            ))
                    ))
                };
            }
        }

        [Theory]
        [MemberData(nameof(VisitInsertIntoQueryCases))]
        public void VisitInsertIntoQuery(InsertIntoQuery insertIntoQuery, Expression<Func<CollectVariableVisitor, bool>> visitorExpectation, Expression<Func<InsertIntoQuery, bool>> insertIntoQueryExpectation)
        {
            // Arrange
            CollectVariableVisitor visitor = new CollectVariableVisitor();

            // Act
            visitor.Visit(insertIntoQuery);

            // Assert
            visitor.Should()
                .Match(visitorExpectation);

            insertIntoQuery.Should().Match(insertIntoQueryExpectation);
        }


        public static IEnumerable<object[]> VisitDeleteQueryCases
        {
            get
            {
                yield return new object[]
                {
                    Delete("members").Where("Activity".Field(), NotLike, "%Super hero%"),
                    ((Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                        visitor.Variables.Count() == 1
                        && visitor.Variables.Any(x => x.Name == "p0" && "%Super hero%".Equals(x.Value) && x.Type == VariableType.String)
                    )),
                    ((Expression<Func<DeleteQuery, bool>>)(query =>
                        query.Equals(Delete("members")
                            .Where("Activity".Field(), NotLike, new Variable("p0", VariableType.String, "%Super hero%")))
                    ))
                };
            }
        }

        [Theory]
        [MemberData(nameof(VisitDeleteQueryCases))]
        public void VisitDeleteQuery(DeleteQuery deleteQuery, Expression<Func<CollectVariableVisitor, bool>> visitorExpectation, Expression<Func<DeleteQuery, bool>> queryAfterVisitExpectation)
        {
            // Arrange
            CollectVariableVisitor visitor = new CollectVariableVisitor();

            // Act
            visitor.Visit(deleteQuery);

            // Assert
            visitor.Should()
                .Match(visitorExpectation);

            deleteQuery.Should().Match(queryAfterVisitExpectation);
        }

    }
}
