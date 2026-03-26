using AwesomeAssertions;
using AwesomeAssertions.Extensions;

using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Sorting;
using Queries.Core.Renderers;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Queries.Core.Builders.Fluent;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

using static Queries.Core.Builders.Fluent.QueryBuilder;
using static Queries.Core.Parts.Clauses.ClauseOperator;
using static Queries.Core.Parts.Columns.SelectColumn;
using static Queries.Renderers.Postgres.Builders.Fluent.ReturnBuilder;


namespace Queries.Renderers.Postgres.Tests;

[UnitTest]
[Feature(nameof(PostgresqlRenderer))]
[Feature(nameof(Postgres))]
public class PostgresRendererTest(ITestOutputHelper outputHelper)
{
    public static TheoryData<SelectQuery, PostgresRendererSettings, string> SelectTestCases
    => new()
        {
            {
                Select(UUID()),
                new PostgresRendererSettings { PrettyPrint = false },
                "SELECT gen_random_uuid()"
            },
            {
                Select(1.Literal()),
                new PostgresRendererSettings { PrettyPrint = false },
                "SELECT 1"
            },
            {
                Select(1.Literal()).Union(Select(2.Literal())).Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                "SELECT 1 UNION SELECT 2"
            },
            {
                Select(1.Literal()).Union(Select(2.Literal())).Build(),
                new PostgresRendererSettings{ PrettyPrint = true },
                $"SELECT 1{Environment.NewLine}" +
                $"UNION{Environment.NewLine}" +
                 "SELECT 2"
            },
            {
                Select("*").From(Select(1.Literal()).Union(Select(2.Literal()))).Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                "SELECT * FROM (SELECT 1 UNION SELECT 2)"
            },
            {
                Select("*")
                .From(
                    Select("identifier")
                    .From("identities")
                    .Union(
                        Select("username")
                        .From("members")
                        ).As("logins")
                    ).Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT * FROM (SELECT ""identifier"" FROM ""identities"" UNION SELECT ""username"" FROM ""members"") ""logins"""
            },
            {
                Select("*").From("Table").Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT * FROM ""Table"""
            },
            {
                Select("*".Field()).From("Table")
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT * FROM ""Table"""
            },
            {
                Select("Employees.*").From("Table")
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT ""Employees"".* FROM ""Table"""
            },
            {
                Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"""
            },
            {
                Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"""
            },
            {
                Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                .From("members")
                .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"""
            },
            {
                Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                .From("members")
                .OrderBy(new OrderExpression("firstname"))
                .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"" ORDER BY ""firstname"""
            },
            {
                Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                .From("members")
                .OrderBy(new OrderExpression("firstname", OrderDirection.Descending))
                .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"" ORDER BY ""firstname"" DESC"
            },
            {
                Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                .From("members")
                .OrderBy("firstname".Desc())
                .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"" ORDER BY ""firstname"" DESC"
            },
            {
                Select(Length(Concat("firstname".Field(), " ".Literal(), "lastname".Field())))
                .From("members")
                .OrderBy("firstname".Desc())
                .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT LENGTH(""firstname"" || ' ' || ""lastname"") FROM ""members"" ORDER BY ""firstname"" DESC"
            },
            {
                Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname"))
                    .From("members")
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT ""firstname"" || ' ' || ""lastname"" ""fullname"" FROM ""members"""
            },
            {
                Select(Null("firstname".Field(), "").As("firstname"))
                    .From("members")
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT COALESCE(""firstname"", '') ""firstname"" FROM ""members"""
            },
            {
                Select(Max("age".Field()).As("age maxi"))
                    .From("members")
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT MAX(""age"") ""age maxi"" FROM ""members"""
            },
            {
                Select(Max(Null("age".Field(), 0)).As("age maxi"))
                    .From("members")
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT MAX(COALESCE(""age"", 0)) ""age maxi"" FROM ""members"""
            },
            {
                Select(Min("age".Field()).As("age mini")).From("members")
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT MIN(""age"") ""age mini"" FROM ""members"""
            },
            {
                Select(Min(Null("age".Field(), 0)).As("age mini"))
                    .From("members")
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT MIN(COALESCE(""age"", 0)) ""age mini"" FROM ""members"""
            },
            {
                Select("firstname".Field(), Max("age".Field()).As("age maximum"))
                    .From("members")
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT ""firstname"", MAX(""age"") ""age maximum"" FROM ""members"" GROUP BY ""firstname"""
            },
            {
                Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0, 1)).As("initials"))
                .From("members")
                .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT SUBSTRING(""firstname"" FROM 0 FOR 1) || SUBSTRING(""lastname"" FROM 0 FOR 1) ""initials"" FROM ""members"""
            },
            {
                Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0)).As("initials"))
                .From("members")
                .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT SUBSTRING(""firstname"" FROM 0 FOR 1) || SUBSTRING(""lastname"" FROM 0) ""initials"" FROM ""members"""
            },
            {
                Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0)).As("initials"))
                .From("members")
                .Build(),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"SELECT SUBSTRING(""firstname"" FROM 0 FOR 1) || SUBSTRING(""lastname"" FROM 0) ""initials"" FROM ""members"""
            },
            {
                Select("settings".Field().Json("theme"))
                    .From("members")
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false},
                @"SELECT ""settings"" -> 'theme' FROM ""members"""
            },
            {
                Select("settings".Field().Json("theme").As("preferences"))
                    .From("members")
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false},
                @"SELECT ""settings"" -> 'theme' AS ""preferences"" FROM ""members"""
            },
            {
                Select("*")
                    .From("members")
                    .Where("settings".Field().Json("theme"), EqualTo, "dark")
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false},
                @"SELECT * FROM ""members"" WHERE (""settings"" ->> 'theme' = 'dark')"
            },
            {
                Select("*")
                    .From("members")
                    .Where("dark".Literal(), EqualTo, "settings".Field().Json("theme"))
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false},
                @"SELECT * FROM ""members"" WHERE ('dark' = ""settings"" ->> 'theme')"
            },
            {
                Select("*")
                    .From("members")
                    .Where("settings".Field().Json("theme"), EqualTo, "dark")
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false},
                @"SELECT * FROM ""members"" WHERE (""settings"" ->> 'theme' = 'dark')"
            },
            {
                Select("*")
                    .From("members")
                    .Where("settings".Field().Json("theme").EqualTo("settings".Field().Json("theme")))
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false},
                @"SELECT * FROM ""members"" WHERE (""settings"" -> 'theme' = ""settings"" -> 'theme')"
            },
            {
                Select("*")
                    .From("members")
                    .Where(new CompositeWhereClause{
                        Logic = ClauseLogic.And,
                        Clauses = new IWhereClause[]
                        {
                            "settings".Field().Json("theme").EqualTo("dark"),
                            "name".Field().EqualTo("super-user")
                        }
                    })
                    .Build(),
                new PostgresRendererSettings{ PrettyPrint = false},
                @"SELECT * FROM ""members"" WHERE ((""settings"" ->> 'theme' = 'dark') AND (""name"" = 'super-user'))"
            }
       };

    public static TheoryData<BatchQuery, PostgresRendererSettings, string> BatchTestCases
        => new()
        {
            {
                new BatchQuery(
                    Delete("members").Where("firstname".Field().IsNull()),
                    Select("*").From("members")
                ),
                new PostgresRendererSettings{ PrettyPrint = false },
                $@"DELETE FROM ""members"" WHERE (""firstname"" IS NULL);SELECT * FROM ""members"";"
            },
            {
                new BatchQuery(
                    InsertInto("members").Values(
                        "Firstname".InsertValue("Bruce".Literal()),
                        "Lastname".InsertValue("Wayne".Literal())
                    ),
                    Return()
                ),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"INSERT INTO ""members"" (""Firstname"", ""Lastname"") VALUES ('Bruce', 'Wayne');RETURN ;"
            },
            {
                new BatchQuery(
                    InsertInto("members").Values(
                        "Firstname".InsertValue("Bruce".Literal()),
                        "Lastname".InsertValue("Wayne".Literal())
                    ),
                    Return(0.Literal())
                ),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"INSERT INTO ""members"" (""Firstname"", ""Lastname"") VALUES ('Bruce', 'Wayne');RETURN 0;"
            },
            {
                new BatchQuery(
                    InsertInto("members").Values(
                        "Firstname".InsertValue("Bruce".Literal()),
                        "Lastname".InsertValue("Wayne".Literal())
                    ),
                    Return("Id".Field())
                ),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"INSERT INTO ""members"" (""Firstname"", ""Lastname"") VALUES ('Bruce', 'Wayne');RETURN ""Id"";"
            },
            {
                new BatchQuery(
                    InsertInto("members").Values(
                        "Firstname".InsertValue("Bruce".Literal()),
                        "Lastname".InsertValue("Wayne".Literal())
                    ),
                    Return(Select(Max("Age".Field())).From("members").Build())
                ),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"INSERT INTO ""members"" (""Firstname"", ""Lastname"") VALUES ('Bruce', 'Wayne');RETURN SELECT MAX(""Age"") FROM ""members"";"
            }
        };

    [Theory]
    [MemberData(nameof(SelectTestCases))]
    public void SelectTest(SelectQuery query, PostgresRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    public static TheoryData<IBuild<SelectQuery>, PostgresRendererSettings, CompiledQuery, string> CompileCases
        => new()
        {
            {
                Select("*")
                    .From("members")
                    .Where("Firstname".Field(), In, new StringValues("Bruce", "Bane")),
                new PostgresRendererSettings{ Parametrization = ParametrizationSettings.SkipVariableDeclaration },
                new CompiledQuery(@"SELECT * FROM ""members"" WHERE (""Firstname"" IN (@p0, @p1))",
                    [ new Variable("p0", VariableType.String, "Bruce"),
                        new Variable("p1", VariableType.String, "Bane")
                    ]
                ),
                "the statement contains 2 variables with 2 values"
            },
            {
                Select("*")
                .From(
                    Select("Fullname").From("People").Where("Firstname".Field(), Like, "B%")
                    .Union(
                    Select("Fullname").From("SuperHero").Where("Nickname".Field(), Like, "B%"))
                ),
                new PostgresRendererSettings{ PrettyPrint = false, Parametrization = ParametrizationSettings.SkipVariableDeclaration },
                new( "SELECT * FROM (" +
                        @"SELECT ""Fullname"" FROM ""People"" WHERE (""Firstname"" LIKE @p0) " +
                        "UNION " +
                        @"SELECT ""Fullname"" FROM ""SuperHero"" WHERE (""Nickname"" LIKE @p0)" +
                    ")",
                    [new Variable("p0", VariableType.String, "B%")]
                ),
                "The select statement as two variables with SAME value"
            },
            {
                Select("id", "file_id")
                    .From("documents")
                    .Where(new WhereClause("userAccount".Field(), Like, "vp%")),
                new PostgresRendererSettings{ PrettyPrint = false, Parametrization = ParametrizationSettings.SkipVariableDeclaration },
                new(@"SELECT ""id"", ""file_id"" FROM ""documents"" WHERE (""userAccount"" LIKE @p0)",
                    [new Variable("p0", VariableType.String, "vp%") ]),
                "The select statement as two variables with SAME value"
            },
            {
                Select("id".Field(), "file_id".Field(), new Literal("COUNT(*) OVER()").As("fullcount"))
                    .From("documents")
                    .Where(new CompositeWhereClause()
                    {
                        Logic = ClauseLogic.And,
                        Clauses =
                        [
                            "userAccount".Field().Like("vp%"),
                            "created_on".Field().EqualTo(10.April(2010))
                        ]
                    })
                    .OrderBy("timestamp".Field().Desc())
                    .Paginate(pageIndex: 2, pageSize: 3),
                new PostgresRendererSettings{ PrettyPrint = false, Parametrization = ParametrizationSettings.SkipVariableDeclaration, FieldnameCasingStrategy = FieldnameCasingStrategy.SnakeCase },
                new CompiledQuery(
                    @"SELECT ""id"", ""file_id"", COUNT(*) OVER() AS ""fullcount"" FROM ""documents"" " +
                             @"WHERE ((""user_account"" LIKE @p0) AND (""created_on"" = @p1)) " +
                             @"ORDER BY ""timestamp"" DESC " +
                             "LIMIT 3 OFFSET 3",
                    [
                        new Variable("p0", VariableType.String, "vp%"),
                        new Variable("p1", VariableType.Date, 10.April(2010))
                    ]
                ),
                "The select statement as two variables with SAME value"
            }
        };

    [Theory]
    [MemberData(nameof(CompileCases))]
    public void Compile(SelectQuery query, PostgresRendererSettings settings, CompiledQuery expected, string reason)
    {
        // Arrange
        outputHelper.WriteLine($"{nameof(query)} : '{query}'");

        // Assert
        CompiledQuery actual = query.CompileForPostgres(settings);

        outputHelper.WriteLine($"{nameof(actual)} : '{actual}'");

        // Assert
        actual.Should().Be(expected, reason);
    }

    public static TheoryData<IBuild<SelectQuery>, FieldnameCasingStrategy, string> FieldnameCasingStrategyCases
        => new ()
        {
            {
                Select("FirstName".Field(), "LastName".Field())
                    .From("members"),
                FieldnameCasingStrategy.Default,
                @"SELECT ""FirstName"", ""LastName"" FROM ""members"""
            },
            {
                Select("FirstName".Field(), "LastName".Field())
                    .From("members"),
                FieldnameCasingStrategy.CamelCase,
                @"SELECT ""firstName"", ""lastName"" FROM ""members"""
            },
            {
                Select("FirstName".Field(), "LastName".Field())
                    .From("members"),
                FieldnameCasingStrategy.SnakeCase,
                @"SELECT ""first_name"", ""last_name"" FROM ""members"""
            }
        };

    [Theory]
    [MemberData(nameof(FieldnameCasingStrategyCases))]
    public void CasingStrategy(SelectQuery query, FieldnameCasingStrategy casingStrategy, string expected)
    {
        // Arrange
        PostgresRendererSettings settings = new()
        {
            FieldnameCasingStrategy = casingStrategy
        };

        PostgresqlRenderer renderer = new(settings);

        // Act
        string statement = renderer.Render(query);

        // Assert
        statement.Should()
            .Be(expected);
    }

    public static TheoryData<UpdateQuery, PostgresRendererSettings, string> UpdateTestCases
        => new()
        {
            {
                Update("members").Set("firstname".Field().UpdateValueTo("")).Where("firstname".Field().IsNull()),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"UPDATE ""members"" SET ""firstname"" = '' WHERE (""firstname"" IS NULL)"
            },
            {
                Update("members").Set("firstname".Field().UpdateValueTo(null)).Where(new WhereClause("firstname".Field(), EqualTo, "")),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"UPDATE ""members"" SET ""firstname"" = NULL WHERE (""firstname"" = '')"
            }
        };

    [Theory]
    [MemberData(nameof(UpdateTestCases))]
    public void UpdateTest(UpdateQuery query, PostgresRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    public static TheoryData<IBuild<DeleteQuery>, PostgresRendererSettings, string> DeleteTestCases
        => new ()
        {
            {
                Delete("members"),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"DELETE FROM ""members"""
            },
            {
                Delete("members").Where(new WhereClause("firstname".Field(), IsNull)),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"DELETE FROM ""members"" WHERE (""firstname"" IS NULL)"
            }
        };

    [Theory]
    [MemberData(nameof(DeleteTestCases))]
    public void DeleteTest(DeleteQuery query, PostgresRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    public static TheoryData<SelectIntoQuery, PostgresRendererSettings, string> SelectIntoTestCases
        => new()
        {
            {
                SelectInto("destination").From("source".Table()).Build(),
                new PostgresRendererSettings { PrettyPrint = false },
                @"SELECT * INTO ""destination"" FROM ""source"""
            },
            {
                SelectInto("names")
                    .From(
                        Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname"))
                            .From("members")
                    )
                    .Build(),
                new PostgresRendererSettings { PrettyPrint = false },
                @"SELECT * INTO ""names"" FROM (SELECT ""firstname"" || ' ' || ""lastname"" ""fullname"" FROM ""members"")"
            },
            {
                SelectInto("names")
                    .From(
                        Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname"))
                            .From("members")
                            .Where("firstname".Field().IsNotNull()))
                    .Build(),
                new PostgresRendererSettings { PrettyPrint = false },
                @"SELECT * INTO ""names"" FROM (SELECT ""firstname"" || ' ' || ""lastname"" ""fullname"" FROM ""members"" WHERE (""firstname"" IS NOT NULL))"
            }
        };

    [Theory]
    [MemberData(nameof(SelectIntoTestCases))]
    public void SelectIntoTest(SelectIntoQuery query, PostgresRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    public static TheoryData<TruncateQuery, PostgresRendererSettings, string> TruncateTestCases
        => new()
        {
            {
                Truncate("table"),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"TRUNCATE TABLE ""table"""
            }
        };

    [Theory]
    [MemberData(nameof(TruncateTestCases))]
    public void TruncateTest(TruncateQuery query, PostgresRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    public static TheoryData<IBuild<InsertIntoQuery>, PostgresRendererSettings, string> InsertIntoTestCases
        => new()
        {
            {
                InsertInto("members").Values(Select("Bruce".Literal(), "Wayne".Literal(), "Batman".Literal())),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"INSERT INTO ""members"" SELECT 'Bruce', 'Wayne', 'Batman'"
            },
            {
                InsertInto("members").Values(Select("Bruce".Literal(), "Wayne".Literal(), "Batman".Literal())),
                new PostgresRendererSettings{ PrettyPrint = true },
                $@"INSERT INTO ""members"" {Environment.NewLine}SELECT 'Bruce', 'Wayne', 'Batman'"
            },
            {
                InsertInto("members").Values("firstname".InsertValue("Bruce".Literal()), "lastname".InsertValue("Wayne".Literal()), "nickname".InsertValue("Batman".Literal())),
                new PostgresRendererSettings{ PrettyPrint = false },
                @"INSERT INTO ""members"" (""firstname"", ""lastname"", ""nickname"") VALUES ('Bruce', 'Wayne', 'Batman')"
            },
            {
                InsertInto("members").Values("firstname".InsertValue("Bruce".Literal()), "lastname".InsertValue("Wayne".Literal()), "nickname".InsertValue("Batman".Literal())),
                new PostgresRendererSettings{ PrettyPrint = true },
                $@"INSERT INTO ""members"" (""firstname"", ""lastname"", ""nickname"") {Environment.NewLine}VALUES ('Bruce', 'Wayne', 'Batman')"
            }
        };

    [Theory]
    [MemberData(nameof(InsertIntoTestCases))]
    public void InsertIntoTest(InsertIntoQuery query, PostgresRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    [Theory]
    [MemberData(nameof(BatchTestCases))]
    public void BatchQueryTest(BatchQuery query, PostgresRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    public static TheoryData<IPaginatedQuery<SelectQuery>, PostgresRendererSettings, string> PaginateCases
    {
        get
        {
            TheoryData<IPaginatedQuery<SelectQuery>, PostgresRendererSettings, string> data = new()
            {
                {
                    Select("col1")
                        .From("table")
                        .Paginate(pageIndex: 1, pageSize: 10), new PostgresRendererSettings(),
                    @"SELECT ""col1"" FROM ""table"" LIMIT 10" } };

            {
                (int pageIndex, int pageSize) = (2, 10);
                data.Add(
                    Select("col1")
                        .From("table")
                        .Paginate(pageIndex: pageIndex, pageSize: pageSize),
                    new PostgresRendererSettings(),
                    $@"SELECT ""col1"" FROM ""table"" LIMIT {pageSize} OFFSET {pageSize}"
                );
            }
            {
                (int pageIndex, int pageSize) = (3, 10);
                data.Add(
                    Select("col1")
                        .From("table")
                        .Paginate(pageIndex: pageIndex, pageSize: pageSize),
                    new PostgresRendererSettings(),
                    $@"SELECT ""col1"" FROM ""table"" LIMIT {pageSize} OFFSET {pageSize} * {(pageIndex - 1)}"
                );
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(PaginateCases))]
    public void PaginateTest(SelectQuery query, PostgresRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    private void IsQueryOk(IQuery query, PostgresRendererSettings settings, string expectedString)
    {
        outputHelper.WriteLine($"Expected string : {expectedString}");
        query.ForPostgres(settings).Should().Be(expectedString);
    }
}