using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using FluentAssertions.Extensions;
using Queries.Core.Builders;
using Queries.Core.Builders.Fluent;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
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
public class CollectVariableVisitorTests(ITestOutputHelper outputHelper)
{
    private readonly CollectVariableVisitor _sut = new();

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

    public static TheoryData<IBuild<SelectQuery>, Expression<Func<CollectVariableVisitor, bool>>, IBuild<SelectQuery>> VisitSelectQueryCases
        => new()
            {
                {
                    Select("Fullname").From("SuperHero").Where("Nickname".Field(), Like, "Bat%"),
                    visitor => visitor.Variables.Count == 1
                               && visitor.Variables.Once(x => x.Name == "p0" && "Bat%".Equals((string)x.Value) && x.Type == VariableType.String),
                    Select("Fullname")
                        .From("SuperHero")
                        .Where("Nickname".Field(), Like, new Variable("p0", VariableType.String, "Bat%"))
                },
                {
                    Select("Fullname")
                        .From("SuperHero")
                        .Where("Nickname".Field(), In, new StringValues("Batman", "Superman")),
                    visitor =>
                        visitor.Variables.Count == 2
                        && visitor.Variables.Once(x => x.Name == "p0" && "Batman".Equals(x.Value) && x.Type == VariableType.String)
                        && visitor.Variables.Once(x => x.Name == "p1" && "Superman".Equals(x.Value) && x.Type == VariableType.String),
                    Select("Fullname")
                        .From("SuperHero")
                        .Where(
                            "Nickname".Field(),
                            In,
                            new VariableValues(
                                new Variable("p0", VariableType.String, "Batman"),
                                new Variable("p1", VariableType.String, "Superman")
                            )
                        )
                },
                {
                    Select("Fullname")
                        .From("SuperHero")
                        .Where(new CompositeWhereClause
                        {
                            Logic = Or,
                            Clauses =
                            [
                                new WhereClause("Nickname".Field(), Like, "Bat%"),
                                new WhereClause("CanFly".Field(), EqualTo, true)
                            ]
                        }),
                    visitor => visitor.Variables.Count == 2
                               && visitor.Variables.Once(x => x.Name == "p0" && "Bat%".Equals((string)x.Value) && x.Type == VariableType.String)
                               && visitor.Variables.Once(x => x.Name == "p1" && true.Equals((bool)x.Value) && x.Type == VariableType.Boolean),

                    Select("Fullname")
                        .From("SuperHero")
                        .Where(new CompositeWhereClause
                        {
                            Logic = Or,
                            Clauses =
                            [
                                new WhereClause("Nickname".Field(), Like, new Variable("p0", VariableType.String, "Bat%")),
                                new WhereClause("CanFly".Field(), EqualTo, new Variable("p1", VariableType.Boolean, true))
                            ]
                        })
                },
                {
                    Select("Fullname")
                        .From("SuperHero")
                        .Where(new CompositeWhereClause
                        {
                            Logic = And,
                            Clauses =
                            [
                                new WhereClause("DateOfBirth".Field(), ClauseOperator.LessThan, 1.January(1990)),
                                new CompositeWhereClause
                                {
                                    Logic = Or,
                                    Clauses =
                                    [
                                        new WhereClause("Nickname".Field(), Like, "Bat%"),
                                        new WhereClause("CanFly".Field(), EqualTo, true)
                                    ]
                                }
                            ]
                        }),
                    visitor =>
                        visitor.Variables.Count == 3
                        && visitor.Variables.Once(x => x.Name == "p0" && 1.January(1990).Equals(x.Value) && x.Type == Date)
                        && visitor.Variables.Once(x => x.Name == "p1" && "Bat%".Equals(x.Value) && x.Type == VariableType.String)
                        && visitor.Variables.Once(x => x.Name == "p2" && true.Equals(x.Value) && x.Type == VariableType.Boolean),
                    Select("Fullname")
                        .From("SuperHero")
                        .Where(
                            new CompositeWhereClause
                            {
                                Logic = And,
                                Clauses =
                                [
                                    new WhereClause("DateOfBirth".Field(), ClauseOperator.LessThan, new Variable("p0", Date, 1.January(1990))),
                                    new CompositeWhereClause
                                    {
                                        Logic = Or,
                                        Clauses =
                                        [
                                            new WhereClause("Nickname".Field(), Like, new Variable("p1", VariableType.String, "Bat%")),
                                            new WhereClause("CanFly".Field(), EqualTo, new Variable("p2", VariableType.Boolean, true))
                                        ]
                                    }
                                ]
                            })
                },

#if NET8_0_OR_GREATER
                {
                    Select("Fullname")
                        .From("SuperHero")
                        .Where(new CompositeWhereClause
                        {
                            Logic = And,
                            Clauses =
                            [
                                new WhereClause("DateOfBirth".Field(), ClauseOperator.LessThan, DateOnly.FromDateTime(1.January(1990))),
                                new CompositeWhereClause
                                {
                                    Logic = Or,
                                    Clauses =
                                    [
                                        new WhereClause("Nickname".Field(), Like, "Bat%"),
                                        new WhereClause("CanFly".Field(), EqualTo, true)
                                    ]
                                }
                            ]
                        }),
                    visitor =>
                        visitor.Variables.Exactly(3)
                        && visitor.Variables.Once(x => x.Name == "p0" && DateOnly.FromDateTime(1.January(1990)).Equals((DateOnly)x.Value) && x.Type == Date)
                        && visitor.Variables.Once(x => x.Name == "p1" && "Bat%".Equals((string)x.Value) && x.Type == VariableType.String)
                        && visitor.Variables.Once(x => x.Name == "p2" && true.Equals((bool)x.Value) && x.Type == VariableType.Boolean),
                    Select("Fullname")
                        .From("SuperHero")
                        .Where(
                            new CompositeWhereClause
                            {
                                Logic = And,
                                Clauses =
                                [
                                    new WhereClause("DateOfBirth".Field(), ClauseOperator.LessThan, new Variable("p0", Date, DateOnly.FromDateTime(1.January(1990)))),
                                    new CompositeWhereClause
                                    {
                                        Logic = Or,
                                        Clauses =
                                        [
                                            new WhereClause("Nickname".Field(), Like, new Variable("p1", VariableType.String, "Bat%")),
                                            new WhereClause("CanFly".Field(), EqualTo, new Variable("p2", VariableType.Boolean, true))
                                        ]
                                    }
                                ]
                            })
                },
                {
                    Select("Fullname")
                        .From("SuperHero")
                        .Where(new CompositeWhereClause
                        {
                            Logic = And,
                            Clauses =
                            [
                                new WhereClause("TimeOfTheDay".Field(), ClauseOperator.LessThan, TimeOnly.FromDateTime(1.January(1990).Add(18.Hours().And(43.Minutes())))),
                                new CompositeWhereClause
                                {
                                    Logic = Or,
                                    Clauses =
                                    [
                                        new WhereClause("Nickname".Field(), Like, "Bat%"),
                                        new WhereClause("CanFly".Field(), EqualTo, true)
                                    ]
                                }
                            ]
                        }),
                    visitor =>
                        visitor.Variables.Exactly(3)
                        && visitor.Variables.Once(x => x.Name == "p0" && TimeOnly.FromDateTime(1.January(1990).Add(18.Hours().And(43.Minutes()))).Equals(x.Value) && x.Type == Time)
                        && visitor.Variables.Once(x => x.Name == "p1" && "Bat%".Equals(x.Value) && x.Type == VariableType.String)
                        && visitor.Variables.Once(x => x.Name == "p2" && true.Equals(x.Value) && x.Type == VariableType.Boolean),
                    Select("Fullname")
                        .From("SuperHero")
                        .Where(
                            new CompositeWhereClause
                            {
                                Logic = And,
                                Clauses =
                                [
                                    new WhereClause("TimeOfTheDay".Field(), ClauseOperator.LessThan, new Variable("p0", Time, TimeOnly.FromDateTime(1.January(1990).Add(18.Hours().And(43.Minutes()))))),
                                    new CompositeWhereClause
                                    {
                                        Logic = Or,
                                        Clauses =
                                        [
                                            new WhereClause("Nickname".Field(), Like, new Variable("p1", VariableType.String, "Bat%")),
                                            new WhereClause("CanFly".Field(), EqualTo, new Variable("p2", VariableType.Boolean, true))
                                        ]
                                    }
                                ]
                            })
                },
#endif
                {
                    Select("*")
                        .From(
                            Select("Fullname").From("People").Where("Firstname".Field(), Like, "B%")
                                .Union(
                                    Select("Fullname").From("Superhero").Where("Nickname".Field(), Like, "B%"))
                        ),
                    visitor => visitor.Variables.Count == 1 && visitor.Variables.Once(x => x.Name == "p0" && "B%".Equals(x.Value) && x.Type == VariableType.String),
                        Select("*")
                            .From(
                                Select("Fullname").From("People").Where("Firstname".Field(), Like, new Variable("p0", VariableType.String, "B%"))
                                    .Union(
                                        Select("Fullname").From("Superhero").Where("Nickname".Field(), Like, new Variable("p0", VariableType.String, "B%")))
                            )
                },
                {
                    Select("Firstname".Field(),
                            "Lastname".Field(),
                            Cases(
                                When("Age".Field().GreaterThan(18), then: true),
                                When("Age".Field().IsNull(), then: false)
                            ).As("IsMajor"))
                        .From("members"),
                    visitor => visitor.Variables.Count == 3
                               && visitor.Variables.Any(x => x.Name == "p0" && 18.Equals(x.Value) && x.Type == Numeric)
                               && visitor.Variables.Any(x => x.Name == "p1" && true.Equals(x.Value) && x.Type == VariableType.Boolean)
                               && visitor.Variables.Any(x => x.Name == "p2" && false.Equals(x.Value) && x.Type == VariableType.Boolean),
                    Select(
                            "Firstname".Field(),
                            "Lastname".Field(),
                            Cases(
                                When("Age".Field().GreaterThan(new Variable("p0", Numeric, 18)), new Variable("p1", VariableType.Boolean, true)),
                                When("Age".Field().IsNull(), new Variable("p2", VariableType.Boolean, false))
                            ).As("IsMajor"))
                        .From("members")
                },
                {
                    Select("Firstname".Field(),
                            "Lastname".Field(),
                            Cases(
                                When("Age".Field().GreaterThan(18), then: true),
                                When("Age".Field().IsNull(), then: false)
                            ).As("IsMajor"))
                        .From("members"),
                    visitor =>
                        visitor.Variables.Count == 3
                        && visitor.Variables.Once(x => x.Name == "p0" && 18.Equals(x.Value) && x.Type == Numeric)
                        && visitor.Variables.Once(x => x.Name == "p1" && true.Equals(x.Value) && x.Type == VariableType.Boolean)
                        && visitor.Variables.Once(x => x.Name == "p2" && false.Equals(x.Value) && x.Type == VariableType.Boolean),
                    Select(
                            "Firstname".Field(),
                            "Lastname".Field(),
                            Cases(
                                When("Age".Field().GreaterThan(new Variable("p0", Numeric, 18)), new Variable("p1", VariableType.Boolean, true)),
                                When("Age".Field().IsNull(), new Variable("p2", VariableType.Boolean, false))
                            ).As("IsMajor"))
                        .From("members")

                }
            };

    public static TheoryData<SelectQuery, Expression<Func<CollectVariableVisitor, bool>>, SelectQuery> VisitPaginateQueryCases
        => new()
        {
            {
                Select("*")
                    .From("table")
                    .Paginate(pageIndex: 1, pageSize: 1),
                visitor => visitor.Variables.Count == 0,
                Select("*").From("table")
                    .Paginate(1, 1)
            }
        };

    [Theory]
    [MemberData(nameof(VisitPaginateQueryCases))]
    public void VisitPaginateQuery(SelectQuery selectQuery,
                                   Expression<Func<CollectVariableVisitor, bool>> visitorExpectation,
                                   SelectQuery expected)
        => VisitSelectQuery(selectQuery, visitorExpectation, expected);

    /// <summary>
    /// Tests <see cref="CollectVariableVisitor.Visit(SelectQuery)"/>.
    /// </summary>
    /// <param name="selectQuery"><see cref="SelectQuery"/> to visit.</param>
    /// <param name="visitorExpectation">_sut' state after  visiting <paramref name="selectQuery"/></param>
    /// <param name="expected"><paramref name="selectQuery"/>' state after being visited</param>
    [Theory]
    [MemberData(nameof(VisitSelectQueryCases))]
    public void VisitSelectQuery(SelectQuery selectQuery, Expression<Func<CollectVariableVisitor, bool>> visitorExpectation, SelectQuery expected)
    {
        // Arrange
        outputHelper.WriteLine($"{nameof(selectQuery)} : {selectQuery}");

        // Act
        _sut.Visit(selectQuery);

        // Assert
        _sut.Should()
            .Match(visitorExpectation);

        selectQuery.Should().BeEquivalentTo(expected);
    }

    public static TheoryData<IWhereClause, Expression<Func<CollectVariableVisitor, bool>>, IWhereClause> VisitWhereCases
        => new()
        {
            {
                "name".Field().Like("Way%"),
                visitor =>
                    visitor.Variables.Count == 1
                    && visitor.Variables.Once(v => v.Name == "p0" && v.Type == VariableType.String && "Way%".Equals(v.Value)),
                "name".Field().Like(new Variable("p0", VariableType.String, "Way%"))
            },

#if NET6_0_OR_GREATER
            {
                "datetime".Field().EqualTo(DateOnly.FromDateTime(23.July(1983))),
                visitor =>
                    visitor.Variables.Count == 1
                    && visitor.Variables.Once(v => v.Name == "p0" && v.Type == Date && DateOnly.FromDateTime(23.July(1983)).Equals((DateOnly)v.Value)),
                "datetime".Field().EqualTo(new Variable("p0", Date, DateOnly.FromDateTime(23.July(1983))))
            },
#endif
            {
                "name".Field().Like("Way%".Literal()),
                visitor =>
                    visitor.Variables.Count == 1
                    && visitor.Variables.Once(v => v.Name == "p0" && v.Type == VariableType.String && "Way%".Equals((string)v.Value)),
                "name".Field().Like(new Variable("p0", VariableType.String, "Way%"))
            },
            {
                "age".Field().LessThan(10),
                visitor => visitor.Variables.Count == 1
                    && visitor.Variables.Once(v => v.Name == "p0" && v.Type == Numeric && 10.Equals((int)v.Value)),
                "age".Field().LessThan(new Variable("p0", Numeric, 10))
            },
            {
                new WhereClause("UserAccount".Field(), Like, "vp%"),
                visitor =>
                    visitor.Variables.Count == 1
                    && visitor.Variables.Once(v => v.Name == "p0" && v.Type == VariableType.String && "vp%".Equals((string)v.Value)),
                "UserAccount".Field().Like(new Variable("p0", VariableType.String, "vp%"))
            }
        };

    [Theory]
    [MemberData(nameof(VisitWhereCases))]
    public void VisitWhere(IWhereClause clause, Expression<Func<CollectVariableVisitor, bool>> visitorExpectation, IWhereClause expected)
    {
        // Arrange
        outputHelper.WriteLine($"{nameof(clause)} : {clause}");

        // Act
        _sut.Visit(clause);

        // Assert
        _sut.Should().Match(visitorExpectation);
        clause.Should().Be(expected);
    }

    public static TheoryData<IBuild<InsertIntoQuery>, Expression<Func<CollectVariableVisitor, bool>>, IBuild<InsertIntoQuery>> VisitInsertIntoQueryCases
        => new()
        {
            {
                InsertInto("SuperHero")
                    .Values(
                        "Firstname".InsertValue("Clark".Literal()),
                        "Lastname".InsertValue("Kent".Literal()),
                        "Powers".InsertValue("Super strength".Literal())),
                visitor =>
                    visitor.Variables.Count == 3
                    && visitor.Variables.Any(x => x.Name == "p0" && "Clark".Equals(x.Value) && x.Type == VariableType.String)
                    && visitor.Variables.Any(x => x.Name == "p1" && "Kent".Equals(x.Value) && x.Type == VariableType.String)
                    && visitor.Variables.Any(x => x.Name == "p2" && "Super strength".Equals(x.Value) && x.Type == VariableType.String),
                InsertInto("SuperHero")
                    .Values(
                        "Firstname".InsertValue(new Variable("p0", VariableType.String, "Clark")),
                        "Lastname".InsertValue(new Variable("p1", VariableType.String, "Kent")),
                        "Powers".InsertValue(new Variable("p2", VariableType.String, "Super strength")))
            }
        };

    [Theory]
    [MemberData(nameof(VisitInsertIntoQueryCases))]
    public void VisitInsertIntoQuery(InsertIntoQuery insertIntoQuery, Expression<Func<CollectVariableVisitor, bool>> visitorExpectation, InsertIntoQuery expected)
    {
        // Act
        _sut.Visit(insertIntoQuery);

        // Assert
        _sut.Should()
            .Match(visitorExpectation);

        insertIntoQuery.Should().Be(expected);
    }

    public static TheoryData<IBuild<DeleteQuery>, Expression<Func<CollectVariableVisitor, bool>>, IBuild<DeleteQuery>> VisitDeleteQueryCases
        => new()
        {
            {
                Delete("members").Where("Activity".Field(), NotLike, "%Super hero%"),
                visitor =>
                    visitor.Variables.Once()
                    && visitor.Variables.Once(x => x.Name == "p0" && "%Super hero%".Equals(x.Value) && x.Type == VariableType.String),
                Delete("members").Where("Activity".Field(), NotLike, new Variable("p0", VariableType.String, "%Super hero%"))
            },
#if NET6_0_OR_GREATER
            {
                Delete("members").Where("LastActivity".Field().GreaterThan(TimeOnly.FromTimeSpan(18.Hours()))),
                (Expression<Func<CollectVariableVisitor, bool>>)(visitor =>
                    visitor.Variables.Once()
                    && visitor.Variables.Once(x => x.Name == "p0"
                                                   && TimeOnly.FromTimeSpan(18.Hours()).Equals(x.Value)
                                                   && x.Type == Time)
                ),
                Delete("members").Where("LastActivity".Field(),
                                                         GreaterThan,
                                                         new Variable("p0", Time, TimeOnly.FromTimeSpan(18.Hours())))
            }
#endif
        };

    [Theory]
    [MemberData(nameof(VisitDeleteQueryCases))]
    public void VisitDeleteQuery(DeleteQuery deleteQuery, Expression<Func<CollectVariableVisitor, bool>> visitorExpectation, DeleteQuery expected)
    {
        // Act
        _sut.Visit(deleteQuery);

        // Assert
        _sut.Should()
            .Match(visitorExpectation);

        deleteQuery.Should().Be(expected);
    }
}