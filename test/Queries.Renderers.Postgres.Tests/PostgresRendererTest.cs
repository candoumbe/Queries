using FluentAssertions;
using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Extensions;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Sorting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using static Queries.Core.Parts.Clauses.ClauseOperator;
using static Queries.Core.Parts.Clauses.ClauseLogic;
using static Queries.Core.Parts.Columns.SelectColumn;
using static Queries.Renderers.Postgres.Builders.Fluent.ReturnBuilder;

namespace Queries.Renderers.Postgres.Tests
{
    [UnitTest]
    [Feature(nameof(PostgresqlRenderer))]
    [Feature(nameof(Postgres))]
    public class PostgresRendererTest
    {
        private readonly ITestOutputHelper _outputHelper;

        public PostgresRendererTest(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public static IEnumerable<object[]> SelectTestCases
        {
            get
            {
                yield return new object[] {
                    Select(UUID()),
                    new PostgresRendererSettings { PrettyPrint = false },
                    "SELECT uuid_generate_v4()" };

                yield return new object[] { Select(1.Literal()),
                    new PostgresRendererSettings { PrettyPrint = false },
                    "SELECT 1"
                };

                yield return new object[] {
                    Select(1.Literal()).Union(Select(2.Literal())),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    "SELECT 1 UNION SELECT 2" };

                yield return new object[] {
                    Select(1.Literal()).Union(Select(2.Literal())),
                    new PostgresRendererSettings{ PrettyPrint = true },
                    $"SELECT 1 {Environment.NewLine}" +
                    $"UNION " +
                    $"{Environment.NewLine}SELECT 2" };

                yield return new object[]
                {
                    Select("*").From(Select(1.Literal()).Union(Select(2.Literal()))),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    "SELECT * FROM (SELECT 1 UNION SELECT 2)"
                };

                yield return new object[] {
                    Select("*")
                    .From(
                        Select("identifier")
                        .From("identities")
                        .Union(
                            Select("username")
                            .From("members")
                            ).As("logins")
                        ),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT * FROM (SELECT ""identifier"" FROM ""identities"" UNION SELECT ""username"" FROM ""members"") ""logins""" };

                yield return new object[]
                {
                    Select("*").From("Table"),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT * FROM ""Table"""
                };

                yield return new object[] {
                    Select("*".Field()).From("Table"),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT * FROM ""Table""" };

                yield return new object[] {
                    Select("Employees.*").From("Table"),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT ""Employees"".* FROM ""Table""" };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field())).From("members"),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field())).From("members"),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    ,
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy(new OrderExpression("firstname")),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"" ORDER BY ""firstname"""
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy(new OrderExpression("firstname", OrderDirection.Descending))
                    ,
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"" ORDER BY ""firstname"" DESC"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    ,
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"" ORDER BY ""firstname"" DESC"
                };

                yield return new object[]
                {
                    Select(Length(Concat("firstname".Field(), " ".Literal(), "lastname".Field())))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    , new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT LENGTH(""firstname"" || ' ' || ""lastname"") FROM ""members"" ORDER BY ""firstname"" DESC"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname")).From("members"),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT ""firstname"" || ' ' || ""lastname"" ""fullname"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Null("firstname".Field(), "").As("firstname")).From("members"),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT COALESCE(""firstname"", '') ""firstname"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Max("age".Field()).As("age maxi")).From("members"),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT MAX(""age"") ""age maxi"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Max(Null("age".Field(), 0)).As("age maxi")).From("members"),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT MAX(COALESCE(""age"", 0)) ""age maxi"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Min("age".Field()).As("age mini")).From("members"),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT MIN(""age"") ""age mini"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Min(Null("age".Field(), 0)).As("age mini")).From("members"),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT MIN(COALESCE(""age"", 0)) ""age mini"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select("firstname".Field(), Max("age".Field()).As("age maximum")).From("members"),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT ""firstname"", MAX(""age"") ""age maximum"" FROM ""members"" GROUP BY ""firstname"""
                };

                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0, 1)).As("initials"))
                    .From("members"),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT SUBSTRING(""firstname"" FROM 0 FOR 1) || SUBSTRING(""lastname"" FROM 0 FOR 1) ""initials"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0)).As("initials"))
                    .From("members"),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT SUBSTRING(""firstname"" FROM 0 FOR 1) || SUBSTRING(""lastname"" FROM 0) ""initials"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0)).As("initials"))
                    .From("members"),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT SUBSTRING(""firstname"" FROM 0 FOR 1) || SUBSTRING(""lastname"" FROM 0) ""initials"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select("settings".Field().Json("theme"))
                        .From("members"),
                    new PostgresRendererSettings{ PrettyPrint = false},
                    @"SELECT ""settings"" -> 'theme' FROM ""members"""
                };

                yield return new object[]
                {
                    Select("settings".Field().Json("theme").As("preferences"))
                        .From("members"),
                    new PostgresRendererSettings{ PrettyPrint = false},
                    @"SELECT ""settings"" -> 'theme' AS ""preferences"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select("*")
                        .From("members")
                        .Where("settings".Field().Json("theme"), EqualTo, "dark"),
                    new PostgresRendererSettings{ PrettyPrint = false},
                    @"SELECT * FROM ""members"" WHERE (""settings"" ->> 'theme' = 'dark')"
                };

                yield return new object[]
                {
                    Select("*")
                        .From("members")
                        .Where("dark".Literal(), EqualTo, "settings".Field().Json("theme")),
                    new PostgresRendererSettings{ PrettyPrint = false},
                    @"SELECT * FROM ""members"" WHERE ('dark' = ""settings"" ->> 'theme')"
                };

                yield return new object[]
                {
                    Select("*")
                        .From("members")
                        .Where("settings".Field().Json("theme"), EqualTo, "dark"),
                    new PostgresRendererSettings{ PrettyPrint = false},
                    @"SELECT * FROM ""members"" WHERE (""settings"" ->> 'theme' = 'dark')"
                };

                yield return new object[]
                {
                    Select("*")
                        .From("members")
                        .Where("settings".Field().Json("theme").EqualTo("settings".Field().Json("theme"))),
                    new PostgresRendererSettings{ PrettyPrint = false},
                    @"SELECT * FROM ""members"" WHERE (""settings"" -> 'theme' = ""settings"" -> 'theme')"
                };

                yield return new object[]
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
                        }),
                    new PostgresRendererSettings{ PrettyPrint = false},
                    @"SELECT * FROM ""members"" WHERE ((""settings"" ->> 'theme' = 'dark') AND (""name"" = 'super-user'))"
                };
            }
        }

        public static IEnumerable<object[]> SelectIntoTestCases
        {
            get
            {
                yield return new object[]
                {
                    SelectInto("destination").From("source".Table()),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT * INTO ""destination"" FROM ""source"""
                };

                yield return new object[]
                {
                    SelectInto("names")
                        .From(
                            Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname"))
                            .From("members")
                    ),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT * INTO ""names"" FROM (SELECT ""firstname"" || ' ' || ""lastname"" ""fullname"" FROM ""members"")"
                };

                yield return new object[]
                {
                    SelectInto("names")
                        .From(
                            Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname"))
                                .From("members")
                                .Where("firstname".Field().IsNotNull())),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"SELECT * INTO ""names"" FROM (SELECT ""firstname"" || ' ' || ""lastname"" ""fullname"" FROM ""members"" WHERE (""firstname"" IS NOT NULL))"
                };
            }
        }

        public static IEnumerable<object[]> UpdateTestCases
        {
            get
            {
                yield return new object[]
                {
                    Update("members").Set("firstname".Field().UpdateValueTo("")).Where("firstname".Field().IsNull()),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"UPDATE ""members"" SET ""firstname"" = '' WHERE (""firstname"" IS NULL)"
                };
                yield return new object[]
                {
                     Update("members").Set("firstname".Field().UpdateValueTo(null)).Where(new WhereClause("firstname".Field(), EqualTo, "")),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"UPDATE ""members"" SET ""firstname"" = NULL WHERE (""firstname"" = '')"
                };
            }
        }

        public static IEnumerable<object[]> DeleteTestCases
        {
            get
            {
                yield return new object[]
                {
                    Delete("members"),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"DELETE FROM ""members"""
                };

                yield return new object[]
                {
                    Delete("members").Where(new WhereClause("firstname".Field(), IsNull)),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"DELETE FROM ""members"" WHERE (""firstname"" IS NULL)"
                };
            }
        }

        public static IEnumerable<object[]> TruncateTestCases
        {
            get
            {
                yield return new object[]
                {
                    Truncate("table"),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"TRUNCATE TABLE ""table"""
                };
            }
        }

        public static IEnumerable<object[]> InsertIntoTestCases
        {
            get
            {
                yield return new object[]
                {
                    InsertInto("members").Values(Select("Bruce".Literal(), "Wayne".Literal(), "Batman".Literal())),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"INSERT INTO ""members"" SELECT 'Bruce', 'Wayne', 'Batman'"
                };

                yield return new object[]
                {
                    InsertInto("members").Values(Select("Bruce".Literal(), "Wayne".Literal(), "Batman".Literal())),
                    new PostgresRendererSettings{ PrettyPrint = true },
                    $@"INSERT INTO ""members"" {Environment.NewLine}SELECT 'Bruce', 'Wayne', 'Batman'"
                };

                yield return new object[]
                {
                    InsertInto("members").Values("firstname".InsertValue("Bruce".Literal()), "lastname".InsertValue("Wayne".Literal()), "nickname".InsertValue("Batman".Literal())),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    @"INSERT INTO ""members"" (""firstname"", ""lastname"", ""nickname"") VALUES ('Bruce', 'Wayne', 'Batman')"
                };

                yield return new object[]
                {
                    InsertInto("members").Values("firstname".InsertValue("Bruce".Literal()), "lastname".InsertValue("Wayne".Literal()), "nickname".InsertValue("Batman".Literal())),
                    new PostgresRendererSettings{ PrettyPrint = true },
                    $@"INSERT INTO ""members"" (""firstname"", ""lastname"", ""nickname"") {Environment.NewLine}VALUES ('Bruce', 'Wayne', 'Batman')"
                };
            }
        }

        public static IEnumerable<object[]> BatchTestCases
        {
            get
            {
                yield return new object[]
                {
                    new BatchQuery(
                        Delete("members").Where("firstname".Field().IsNull()),
                        Select("*").From("members")
                    ),
                    new PostgresRendererSettings{ PrettyPrint = false },
                    $@"DELETE FROM ""members"" WHERE (""firstname"" IS NULL);SELECT * FROM ""members"";"
                };

                yield return new object[]
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
                };

                yield return new object[]
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
                };

                yield return new object[]
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
                };

                yield return new object[]
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
                };
            }
        }

        [Theory]
        [MemberData(nameof(SelectTestCases))]
        public void SelectTest(SelectQuery query, PostgresRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        public static IEnumerable<object[]> CompileCases
        {
            get
            {
                yield return new object[]
                {
                    Select("*").From("members").Where("Firstname".Field(), In, new StringValues("Bruce", "Bane")),
                    new PostgresRendererSettings{ SkipVariableDeclaration = true },
                    (Expression<Func<CompiledQuery, bool>>)(
                        query => query.Statement == @"SELECT * FROM ""members"" WHERE (""Firstname"" IN (@p0, @p1))"
                            && query.Variables.Exactly(2)
                            && query.Variables.Once(v => v.Name == "p0" && "Bruce".Equals(v.Value))
                            && query.Variables.Once(v => v.Name == "p1" && "Bane".Equals(v.Value))
                    ),
                    "the statement contains 2 variables with 2 values"
                };

                yield return new object[]
                {
                    Select("*")
                    .From(
                        Select("Fullname").From("People").Where("Firstname".Field(), Like, "B%")
                        .Union(
                        Select("Fullname").From("SuperHero").Where("Nickname".Field(), Like, "B%"))
                    ),
                    new PostgresRendererSettings{ PrettyPrint = false, SkipVariableDeclaration = true },
                    (Expression<Func<CompiledQuery, bool>>)(
                        query => query.Statement == "SELECT * FROM (" +
                            @"SELECT ""Fullname"" FROM ""People"" WHERE (""Firstname"" LIKE @p0) " +
                            "UNION " +
                            @"SELECT ""Fullname"" FROM ""SuperHero"" WHERE (""Nickname"" LIKE @p0)" +
                        ")"
                            && query.Variables.Once()
                            && query.Variables.Once(v => v.Name == "p0" && "B%".Equals(v.Value))
                    ),
                    "The select statement as two variables with SAME value"
                };

                yield return new object[]
                {
                    Select("id", "file_id")
                        .From("documents")
                        .Where(new WhereClause("userAccount".Field(), Like, "anAccount%".Literal())),
                    new PostgresRendererSettings{ PrettyPrint = false, SkipVariableDeclaration = true },
                    (Expression<Func<CompiledQuery, bool>>)(
                        query => query.Statement == @"SELECT ""id"", ""file_id"" FROM ""documents"" WHERE (""userAccount"" LIKE @p0)"
                            && query.Variables.Exactly(1)
                            && query.Variables.Once(v => v.Name == "p0" && "anAccount%".Equals(v.Value) && v.Type == VariableType.String)
                    ),
                    "The select statement as two variables with SAME value"
                };
            }
        }

        [Theory]
        [MemberData(nameof(CompileCases))]
        public void Compile(SelectQuery query, PostgresRendererSettings settings, Expression<Func<CompiledQuery, bool>> expectation, string reason)
        {
            // Arrange
            PostgresqlRenderer renderer = new PostgresqlRenderer(settings);

            // Assert
            CompiledQuery compiledQuery = renderer.Compile(query);

            _outputHelper.WriteLine($"{nameof(compiledQuery)} : '{compiledQuery}'");

            // Assert
            compiledQuery.Should()
                .Match(expectation, reason);
        }

        [Theory]
        [MemberData(nameof(UpdateTestCases))]
        public void UpdateTest(UpdateQuery query, PostgresRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        [Theory]
        [MemberData(nameof(DeleteTestCases))]
        public void DeleteTest(DeleteQuery query, PostgresRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        [Theory]
        [MemberData(nameof(SelectIntoTestCases))]
        public void SelectIntoTest(SelectIntoQuery query, PostgresRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        [Theory]
        [MemberData(nameof(TruncateTestCases))]
        public void TruncateTest(TruncateQuery query, PostgresRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        [Theory]
        [MemberData(nameof(InsertIntoTestCases))]
        public void InsertIntoTest(InsertIntoQuery query, PostgresRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        [Theory]
        [MemberData(nameof(BatchTestCases))]
        public void BatchQueryTest(BatchQuery query, PostgresRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        private void IsQueryOk(IQuery query, PostgresRendererSettings settings, string expectedString)
        {
            _outputHelper.WriteLine($"Expected string : {expectedString}");
            query.ForPostgres(settings).Should().Be(expectedString);
        }
    }
}