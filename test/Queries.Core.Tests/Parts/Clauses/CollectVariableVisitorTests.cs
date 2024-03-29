﻿using FluentAssertions;
using FluentAssertions.Extensions;

using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

using static Queries.Core.Builders.Fluent.QueryBuilder;
using static Queries.Core.Parts.Clauses.ClauseLogic;
using static Queries.Core.Parts.Clauses.ClauseOperator;
using static Queries.Core.Parts.Clauses.VariableType;

namespace Queries.Core.Tests.Parts.Clauses;

[UnitTest]
[Feature("Parameterized query")]
public class CollectVariableVisitorTests
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly CollectVariableVisitor _sut;

    public CollectVariableVisitorTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _sut = new();
    }

    [Fact]
    public void CtorShouldBuildValidInstance()
    {
        // Act
        CollectVariableVisitor instance = new();

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
                (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                    visitor.Variables.Once()
                    && visitor.Variables.Once(x => x.Name == "p0" && "Bat%".Equals(x.Value) && x.Type == VariableType.String)
                ),
                (Expression<Func<SelectQuery, bool>>)(query =>
                    query.Equals(Select("Fullname").From("SuperHero")
                                                   .Where("Nickname".Field(), Like, new Variable("p0", VariableType.String, "Bat%"))))
            };

            yield return new object[]
            {
                Select("Fullname").From("SuperHero").Where("Nickname".Field(), In, new StringValues("Batman", "Superman")),
                (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                    visitor.Variables.Exactly(2)
                    && visitor.Variables.Any(x => x.Name == "p0" && "Batman".Equals(x.Value) && x.Type == VariableType.String)
                    && visitor.Variables.Any(x => x.Name == "p1" && "Superman".Equals(x.Value) && x.Type == VariableType.String)
                ),
                (Expression<Func<SelectQuery, bool>>)(query =>
                    query.Equals(
                        Select("Fullname")
                        .From("SuperHero")
                        .Where(
                            "Nickname".Field(),
                            In,
                            new VariableValues(
                                new Variable("p0", VariableType.String, "Batman"),
                                new Variable("p1", VariableType.String, "Superman")
                            )
                        )))
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
                (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                    visitor.Variables.Exactly(2)
                    && visitor.Variables.Once(x => x.Name == "p0" && "Bat%".Equals(x.Value) && x.Type == VariableType.String)
                    && visitor.Variables.Once(x => x.Name == "p1" && true.Equals(x.Value) && x.Type == VariableType.Boolean)
                ),
                (Expression<Func<SelectQuery, bool>>)(query =>
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
            };

            yield return new object[]
            {
                Select("Fullname").From("SuperHero")
                    .Where(new CompositeWhereClause
                        {
                            Logic = And,
                            Clauses = new IWhereClause[]
                            {
                                new WhereClause("DateOfBirth".Field(), ClauseOperator.LessThan, 1.January(1990)),
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
                (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                    visitor.Variables.Exactly(3)
                    && visitor.Variables.Once(x => x.Name == "p0" && 1.January(1990).Equals(x.Value) && x.Type == Date)
                    && visitor.Variables.Once(x => x.Name == "p1" && "Bat%".Equals(x.Value) && x.Type == VariableType.String)
                    && visitor.Variables.Once(x => x.Name == "p2" && true.Equals(x.Value) && x.Type == VariableType.Boolean)
                ),
                (Expression<Func<SelectQuery, bool>>)(query =>
                    query.Equals(
                        Select("Fullname")
                        .From("SuperHero")
                        .Where(
                            new CompositeWhereClause
                            {
                                Logic = And,
                                Clauses = new IWhereClause[]
                                {
                                    new WhereClause("DateOfBirth".Field(), ClauseOperator.LessThan, new Variable("p0", VariableType.Date, 1.January(1990))),
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
            };

#if NET6_0_OR_GREATER
            yield return new object[]
            {
                Select("Fullname").From("SuperHero")
                    .Where(new CompositeWhereClause
                        {
                            Logic = And,
                            Clauses = new IWhereClause[]
                            {
                                new WhereClause("DateOfBirth".Field(), ClauseOperator.LessThan, DateOnly.FromDateTime(1.January(1990))),
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
                (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                    visitor.Variables.Exactly(3)
                    && visitor.Variables.Once(x => x.Name == "p0" && DateOnly.FromDateTime(1.January(1990)).Equals(x.Value) && x.Type == Date)
                    && visitor.Variables.Once(x => x.Name == "p1" && "Bat%".Equals(x.Value) && x.Type == VariableType.String)
                    && visitor.Variables.Once(x => x.Name == "p2" && true.Equals(x.Value) && x.Type == VariableType.Boolean)
                ),
                (Expression<Func<SelectQuery, bool>>)(query =>
                    query.Equals(
                        Select("Fullname")
                        .From("SuperHero")
                        .Where(
                            new CompositeWhereClause
                            {
                                Logic = And,
                                Clauses = new IWhereClause[]
                                {
                                    new WhereClause("DateOfBirth".Field(), ClauseOperator.LessThan, new Variable("p0", Date, DateOnly.FromDateTime(1.January(1990)))),
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
            };

            yield return new object[]
            {
                Select("Fullname").From("SuperHero")
                    .Where(new CompositeWhereClause
                        {
                            Logic = And,
                            Clauses = new IWhereClause[]
                            {
                                new WhereClause("TimeOfTheDay".Field(), ClauseOperator.LessThan, TimeOnly.FromDateTime(1.January(1990).Add(18.Hours().And(43.Minutes())))),
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
                (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                    visitor.Variables.Exactly(3)
                    && visitor.Variables.Once(x => x.Name == "p0" && TimeOnly.FromDateTime(1.January(1990).Add(18.Hours().And(43.Minutes()))).Equals(x.Value) && x.Type == Time)
                    && visitor.Variables.Once(x => x.Name == "p1" && "Bat%".Equals(x.Value) && x.Type == VariableType.String)
                    && visitor.Variables.Once(x => x.Name == "p2" && true.Equals(x.Value) && x.Type == VariableType.Boolean)
                ),
                (Expression<Func<SelectQuery, bool>>)(query =>
                    query.Equals(
                        Select("Fullname")
                        .From("SuperHero")
                        .Where(
                            new CompositeWhereClause
                            {
                                Logic = And,
                                Clauses = new IWhereClause[]
                                {
                                    new WhereClause("TimeOfTheDay".Field(), ClauseOperator.LessThan, new Variable("p0", Time, TimeOnly.FromDateTime(1.January(1990).Add(18.Hours().And(43.Minutes()))))),
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
            };
#endif

            yield return new object[]
            {
                Select("*")
                .From(
                    Select("Fullname").From("People").Where("Firstname".Field(), Like, "B%")
                    .Union(
                    Select("Fullname").From("Superhero").Where("Nickname".Field(), Like, "B%"))
                ),
                (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                    visitor.Variables.Exactly(1)
                    && visitor.Variables.Once(x => x.Name == "p0" && "B%".Equals(x.Value) && x.Type == VariableType.String)
                ),
                (Expression<Func<SelectQuery, bool>>)(query =>
                    query.Equals(
                        Select("*")
                        .From(
                            Select("Fullname").From("People").Where("Firstname".Field(), Like, new Variable("p0", VariableType.String, "B%"))
                            .Union(
                            Select("Fullname").From("Superhero").Where("Nickname".Field(), Like, new Variable("p0", VariableType.String, "B%")))
                        ))
                )
            };

            yield return new object[]
            {
                Select(
                    "Firstname".Field(),
                    "Lastname".Field(),
                    Cases(
                        When("Age".Field().GreaterThan(18), then : true),
                        When("Age".Field().IsNull(), then : false)
                    ).As("IsMajor"))
                    .From("members"),
               (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                   visitor.Variables.Exactly(3)
                   && visitor.Variables.Any(x => x.Name == "p0" && 18.Equals(x.Value) && x.Type == Numeric)
                   && visitor.Variables.Any(x => x.Name == "p1" && true.Equals(x.Value) && x.Type == VariableType.Boolean)
                   && visitor.Variables.Any(x => x.Name == "p2" && false.Equals(x.Value) && x.Type == VariableType.Boolean)
               ),
                (Expression<Func<SelectQuery, bool>>)(query =>
                    query.Equals(
                        Select(
                                "Firstname".Field(),
                                "Lastname".Field(),
                                Cases(
                                    When("Age".Field().GreaterThan(new Variable("p0", Numeric, 18)), new Variable("p1", VariableType.Boolean, true)),
                                    When("Age".Field().IsNull(), new Variable("p2", VariableType.Boolean, false))
                                ).As("IsMajor"))
                            .From("members")
                    )
                )
            };

            yield return new object[]
            {
                Select(
                    "Firstname".Field(),
                    "Lastname".Field(),
                    Cases(
                        When("Age".Field().GreaterThan(18), then : true),
                        When("Age".Field().IsNull(), then : false)
                    ).As("IsMajor"))
                    .From("members"),
               (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                   visitor.Variables.Exactly(3)
                   && visitor.Variables.Once(x => x.Name == "p0" && 18.Equals(x.Value) && x.Type == Numeric)
                   && visitor.Variables.Once(x => x.Name == "p1" && true.Equals(x.Value) && x.Type == VariableType.Boolean)
                   && visitor.Variables.Once(x => x.Name == "p2" && false.Equals(x.Value) && x.Type == VariableType.Boolean)
               ),
                (Expression<Func<SelectQuery, bool>>)(query =>
                    query.Equals(
                        Select(
                                "Firstname".Field(),
                                "Lastname".Field(),
                                Cases(
                                    When("Age".Field().GreaterThan(new Variable("p0", Numeric, 18)), new Variable("p1", VariableType.Boolean, true)),
                                    When("Age".Field().IsNull(), new Variable("p2", VariableType.Boolean, false))
                                ).As("IsMajor"))
                            .From("members")
                    )
                )
            };
        }
    }

    public static IEnumerable<object[]> VisitPaginateQueryCases
    {
        get
        {
            yield return new object[]
            {
                Select("*")
                    .From("table")
                    .Paginate(pageIndex: 1, pageSize: 1),
                (Expression<Func<CollectVariableVisitor, bool>> )(visitor => visitor.Variables.Exactly(0)),
                (Expression<Func<SelectQuery, bool>>)(query => query.Equals(Select("*").From("table")
                                                                                       .Paginate( 1, 1)
                        )
                    )
            };
        }
    }

    [Theory]
    [MemberData(nameof(VisitPaginateQueryCases))]
    public void VisitPaginateQuery(SelectQuery selectQuery,
        Expression<Func<CollectVariableVisitor, bool>> visitorExpectation,
        Expression<Func<SelectQuery, bool>> selectQueryExpectation)
        => VisitSelectQuery(selectQuery, visitorExpectation, selectQueryExpectation);

    /// <summary>
    /// Tests <see cref="CollectVariableVisitor.Visit(SelectQuery)"/>.
    /// </summary>
    /// <param name="selectQuery"><see cref="SelectQuery"/> to visit.</param>
    /// <param name="visitorExpectation">_sut' state after  visiting <paramref name="selectQuery"/></param>
    /// <param name="selectQueryExpectation"><paramref name="selectQuery"/>' state after being visited</param>
    [Theory]
    [MemberData(nameof(VisitSelectQueryCases))]
    public void VisitSelectQuery(SelectQuery selectQuery, Expression<Func<CollectVariableVisitor, bool>> visitorExpectation, Expression<Func<SelectQuery, bool>> selectQueryExpectation)
    {
        // Arrange
        _outputHelper.WriteLine($"{nameof(selectQuery)} : {selectQuery}");

        // Act
        _sut.Visit(selectQuery);

        // Assert
        _sut.Should()
            .Match(visitorExpectation);

        selectQuery.Should().Match(selectQueryExpectation);
    }

    public static IEnumerable<object[]> VisitWhereCases
    {
        get
        {
            yield return new object[]
            {
                "name".Field().Like("Way%"),
                (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                    visitor.Variables.Once()
                    && visitor.Variables.Once(v => v.Name == "p0" && v.Type == VariableType.String && "Way%".Equals(v.Value))

                ),
                (Expression<Func<IWhereClause, bool>>)(clause =>
                    clause.Equals("name".Field().Like(new Variable("p0", VariableType.String, "Way%"))))
            };

#if NET6_0_OR_GREATER
            yield return new object[]
            {
                "datetime".Field().EqualTo(DateOnly.FromDateTime(23.July(1983))),
                (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                    visitor.Variables.Once()
                    && visitor.Variables.Once(v => v.Name == "p0" && v.Type == Date && DateOnly.FromDateTime(23.July(1983)).Equals(v.Value))

                ),
                (Expression<Func<IWhereClause, bool>>)(clause =>
                    clause.Equals("datetime".Field().EqualTo(new Variable("p0", Date, DateOnly.FromDateTime(23.July(1983))))))
            };
#endif

            yield return new object[]
            {
                "name".Field().Like("Way%".Literal()),
                (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                    visitor.Variables.Once()
                    && visitor.Variables.Once(v => v.Name == "p0" && v.Type == VariableType.String && "Way%".Equals(v.Value))

                ),
                (Expression<Func<IWhereClause, bool>>)(clause =>
                    clause.Equals("name".Field().Like(new Variable("p0", VariableType.String, "Way%"))))
            };

            yield return new object[]
            {
                "age".Field().LessThan(10),
                (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                    visitor.Variables.Once()
                    && visitor.Variables.Once(v => v.Name == "p0" && v.Type == Numeric && 10.Equals(v.Value))

                ),
                (Expression<Func<IWhereClause, bool>>)(clause =>
                    clause.Equals("age".Field().LessThan(new Variable("p0", Numeric, 10))))
            };

            yield return new object[]
            {
                new WhereClause("UserAccount".Field(), Like, "vp%"),
                (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                    visitor.Variables.Once()
                    && visitor.Variables.Once(v => v.Name == "p0" && v.Type == VariableType.String && "vp%".Equals(v.Value))

                ),
                (Expression<Func<IWhereClause, bool>>)(clause =>
                    clause.Equals("UserAccount".Field().Like(new Variable("p0", VariableType.String, "vp%"))))
            };
        }
    }

    [Theory]
    [MemberData(nameof(VisitWhereCases))]
    public void VisitWhere(IWhereClause clause, Expression<Func<CollectVariableVisitor, bool>> visitorExpectation, Expression<Func<IWhereClause, bool>> clauseExpectation)
    {
        // Arrange
        _outputHelper.WriteLine($"{nameof(clause)} : {clause}");

        // Act
        _sut.Visit(clause);

        // Assert
        _sut.Should()
            .Match(visitorExpectation);
        clause.Should()
            .Match(clauseExpectation);
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
                (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                    visitor.Variables.Count() == 3
                    && visitor.Variables.Any(x => x.Name == "p0" && "Clark".Equals(x.Value) && x.Type == VariableType.String)
                    && visitor.Variables.Any(x => x.Name == "p1" && "Kent".Equals(x.Value) && x.Type == VariableType.String)
                    && visitor.Variables.Any(x => x.Name == "p2" && "Super strength".Equals(x.Value) && x.Type == VariableType.String)
                ),
                (Expression<Func<InsertIntoQuery, bool>>)(query =>
                    query.Equals(InsertInto("SuperHero").Values(
                        "Firstname".InsertValue(new Variable("p0", VariableType.String, "Clark")),
                        "Lastname".InsertValue(new Variable("p1", VariableType.String, "Kent")),
                        "Powers".InsertValue(new Variable("p2", VariableType.String, "Super strength"))
                        ))
                )
            };
        }
    }

    [Theory]
    [MemberData(nameof(VisitInsertIntoQueryCases))]
    public void VisitInsertIntoQuery(InsertIntoQuery insertIntoQuery, Expression<Func<CollectVariableVisitor, bool>> visitorExpectation, Expression<Func<InsertIntoQuery, bool>> insertIntoQueryExpectation)
    {
        // Arrange
        CollectVariableVisitor _sut = new();

        // Act
        _sut.Visit(insertIntoQuery);

        // Assert
        _sut.Should()
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
                (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                    visitor.Variables.Once()
                    && visitor.Variables.Once(x => x.Name == "p0" && "%Super hero%".Equals(x.Value) && x.Type == VariableType.String)
                ),
                (Expression<Func<DeleteQuery, bool>>)(query =>
                    query.Table == "members"
                    && query.Criteria.Equals(new WhereClause("Activity".Field(), NotLike, new Variable("p0", VariableType.String, "%Super hero%")))
                )
            };

#if NET6_0_OR_GREATER
            yield return new object[]
            {
                Delete("members").Where("LastActivity".Field().GreaterThan(TimeOnly.FromTimeSpan(18.Hours()))),
                (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                    visitor.Variables.Once()
                    && visitor.Variables.Once(x => x.Name == "p0"
                                                   && TimeOnly.FromTimeSpan(18.Hours()).Equals(x.Value)
                                                   && x.Type == Time)
                ),
                (Expression<Func<DeleteQuery, bool>>)(query =>
                    query.Table == "members"
                    && query.Criteria.Equals(new WhereClause("LastActivity".Field(),
                                                             GreaterThan,
                                                             new Variable("p0", Time, TimeOnly.FromTimeSpan(18.Hours()))))
                )
            };
#endif
        }
    }

    [Theory]
    [MemberData(nameof(VisitDeleteQueryCases))]
    public void VisitDeleteQuery(DeleteQuery deleteQuery, Expression<Func<CollectVariableVisitor, bool>> visitorExpectation, Expression<Func<DeleteQuery, bool>> queryAfterVisitExpectation)
    {
        // Arrange
        CollectVariableVisitor _sut = new();

        // Act
        _sut.Visit(deleteQuery);

        // Assert
        _sut.Should()
            .Match(visitorExpectation);

        deleteQuery.Should().Match(queryAfterVisitExpectation);
    }
}
