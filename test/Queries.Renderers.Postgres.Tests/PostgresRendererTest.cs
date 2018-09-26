using System;
using System.Collections.Generic;
using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Extensions;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Sorting;
using Xunit;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using static Queries.Core.Parts.Clauses.ClauseOperator;
using static Queries.Core.Parts.Columns.SelectColumn;
using FluentAssertions;
using Queries.Core.Parts.Columns;
using Queries.Core.Renderers;

namespace Queries.Renderers.Postgres.Tests
{
    public class PostgresRendererTest
    {
        public static IEnumerable<object[]> SelectTestCases
        {
            get
            {
                yield return new object[] {
                    Select(UUID()),
                    new QueryRendererSettings { PrettyPrint = false },
                    "SELECT uuid_generate_v4()" };

                yield return new object[] { Select(1.Literal()),
                    new QueryRendererSettings { PrettyPrint = false },
                    "SELECT 1"
                };

                yield return new object[] {
                    Select(1.Literal()).Union(Select(2.Literal())),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "SELECT 1 UNION SELECT 2" };

                yield return new object[] {
                    Select(1.Literal()).Union(Select(2.Literal())),
                    new QueryRendererSettings{ PrettyPrint = true },
                    $"SELECT 1 {Environment.NewLine}" +
                    $"UNION " +
                    $"{Environment.NewLine}SELECT 2" };

                yield return new object[]
                {
                    Select("*").From(Select(1.Literal()).Union(Select(2.Literal()))),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "SELECT * FROM (SELECT 1 UNION SELECT 2)"
                };

                yield return new object[] {
                    Select("*")
                    .From(
                        Select("identifier").From("identities").Union(Select("username").From("members")).As("logins")
                        ),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT * FROM (SELECT ""identifier"" FROM ""identities"" UNION SELECT ""username"" FROM ""members"") ""logins""" };

                yield return new object[]
                {
                    Select("*").From("Table"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT * FROM ""Table"""
                };

                yield return new object[] {
                    Select("*".Field()).From("Table"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT * FROM ""Table""" };

                yield return new object[] {
                    Select("Employees.*").From("Table"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT ""Employees"".* FROM ""Table""" };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field())).From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field())).From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    ,
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy(new SortExpression("firstname")),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"" ORDER BY ""firstname"""
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy(new SortExpression("firstname", SortDirection.Descending))
                    ,
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"" ORDER BY ""firstname"" DESC"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    ,
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"" ORDER BY ""firstname"" DESC"
                };

                yield return new object[]
                {
                    Select(Length(Concat("firstname".Field(), " ".Literal(), "lastname".Field())))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    , new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT LENGTH(""firstname"" || ' ' || ""lastname"") FROM ""members"" ORDER BY ""firstname"" DESC"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname")).From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT ""firstname"" || ' ' || ""lastname"" ""fullname"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Null("firstname".Field(), "").As("firstname")).From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT COALESCE(""firstname"", '') ""firstname"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Max("age".Field()).As("age maxi")).From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT MAX(""age"") ""age maxi"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Max(Null("age".Field(), 0)).As("age maxi")).From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT MAX(COALESCE(""age"", 0)) ""age maxi"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Min("age".Field()).As("age mini")).From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT MIN(""age"") ""age mini"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Min(Null("age".Field(), 0)).As("age mini")).From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT MIN(COALESCE(""age"", 0)) ""age mini"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select("firstname".Field(), Max("age".Field()).As("age maximum")).From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT ""firstname"", MAX(""age"") ""age maximum"" FROM ""members"" GROUP BY ""firstname"""
                };

                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0, 1)).As("initials"))
                    .From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT SUBSTRING(""firstname"" FROM 0 FOR 1) || SUBSTRING(""lastname"" FROM 0 FOR 1) ""initials"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0)).As("initials"))
                    .From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT SUBSTRING(""firstname"" FROM 0 FOR 1) || SUBSTRING(""lastname"" FROM 0) ""initials"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0)).As("initials"))
                    .From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT SUBSTRING(""firstname"" FROM 0 FOR 1) || SUBSTRING(""lastname"" FROM 0) ""initials"" FROM ""members"""
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
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT * INTO ""destination"" FROM ""source"""
                };

                yield return new object[]
                {
                    SelectInto("names")
                        .From(
                            Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname"))
                            .From("members")
                    ),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"SELECT * INTO ""names"" FROM (SELECT ""firstname"" || ' ' || ""lastname"" ""fullname"" FROM ""members"")"

                };

                yield return new object[]
                {
                    SelectInto("names")
                        .From(
                            Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname"))
                                .From("members")
                                .Where("firstname".Field().IsNotNull())),
                    new QueryRendererSettings{ PrettyPrint = false },
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
                    Update("members").Set("firstname".Field().EqualTo("")).Where("firstname".Field().IsNull()),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"UPDATE ""members"" SET ""firstname"" = '' WHERE (""firstname"" IS NULL)"
                };
                yield return new object[]
                {
                     Update("members").Set("firstname".Field().EqualTo(null)).Where(new WhereClause("firstname".Field(), EqualTo, "")),
                    new QueryRendererSettings{ PrettyPrint = false },
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
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"DELETE FROM ""members"""
                };

                yield return new object[]
                {
                    Delete("members").Where(new WhereClause("firstname".Field(), IsNull)),
                    new QueryRendererSettings{ PrettyPrint = false },
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
                    new QueryRendererSettings{ PrettyPrint = false },
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
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"INSERT INTO ""members"" SELECT 'Bruce', 'Wayne', 'Batman'"
                };

                yield return new object[]
                {
                    InsertInto("members").Values(Select("Bruce".Literal(), "Wayne".Literal(), "Batman".Literal())),
                    new QueryRendererSettings{ PrettyPrint = true },
                    $@"INSERT INTO ""members"" {Environment.NewLine}SELECT 'Bruce', 'Wayne', 'Batman'"
                };

                yield return new object[]
                {
                    InsertInto("members").Values("firstname".InsertValue("Bruce".Literal()), "lastname".InsertValue("Wayne".Literal()), "nickname".InsertValue("Batman".Literal())),
                    new QueryRendererSettings{ PrettyPrint = false },
                    @"INSERT INTO ""members"" (""firstname"", ""lastname"", ""nickname"") VALUES ('Bruce', 'Wayne', 'Batman')"
                };

                yield return new object[]
                {
                    InsertInto("members").Values("firstname".InsertValue("Bruce".Literal()), "lastname".InsertValue("Wayne".Literal()), "nickname".InsertValue("Batman".Literal())),
                    new QueryRendererSettings{ PrettyPrint = true },
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
                        Delete("members").Where(new WhereClause("firstname".Field(), IsNull)),
                        Select("*").From("members")
                    ),
                    new QueryRendererSettings{ PrettyPrint = false },
                    $@"DELETE FROM ""members"" WHERE (""firstname"" IS NULL);{Environment.NewLine}SELECT * FROM ""members"""
                };
            }
        }

        [Theory]
        [MemberData(nameof(SelectTestCases))]
        public void SelectTest(SelectQuery query, QueryRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        [Theory]
        [MemberData(nameof(UpdateTestCases))]
        public void UpdateTest(UpdateQuery query, QueryRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        [Theory]
        [MemberData(nameof(DeleteTestCases))]
        public void DeleteTest(DeleteQuery query, QueryRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        [Theory]
        [MemberData(nameof(SelectIntoTestCases))]
        public void SelectIntoTest(SelectIntoQuery query, QueryRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        [Theory]
        [MemberData(nameof(TruncateTestCases))]
        public void TruncateTest(TruncateQuery query, QueryRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        [Theory]
        [MemberData(nameof(InsertIntoTestCases))]
        public void InsertIntoTest(InsertIntoQuery query, QueryRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        [Theory]
        [MemberData(nameof(BatchTestCases))]
        public void BatchQueryTest(BatchQuery query, QueryRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        private static void IsQueryOk(IQuery query, QueryRendererSettings settings, string expectedString) =>
            query.ForPostgres(settings).Should().Be(expectedString);
    }
}
