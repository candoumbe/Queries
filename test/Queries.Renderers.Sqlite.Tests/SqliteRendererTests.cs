using FluentAssertions;
using FluentAssertions.Extensions;
using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Parts.Columns;
using Queries.Core.Renderers;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using static Queries.Core.Parts.Clauses.ClauseOperator;

namespace Queries.Renderers.Sqlite.Tests
{
    [UnitTest]
    [Feature("Sqlite")]
    public class SqliteRendererTests
    {
        private ITestOutputHelper _outputHelper;

        public SqliteRendererTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void DefaultConstructor()
        {
            // Act
            SqliteRenderer renderer = new SqliteRenderer();

            // Assert
            renderer.Settings.Should().NotBeNull();
            renderer.Settings.PrettyPrint.Should().BeTrue($"{nameof(SqliteRenderer)}.{nameof(SqliteRenderer.Settings)}.{nameof(SqliteRenderer.Settings.PrettyPrint)} should be set to true by default");
            renderer.Settings.DateFormatString.Should().Be("yyyy-MM-dd");
            renderer.Settings.PaginationKind.Should()
                .Be(PaginationKind.Limit);
        }

        public static IEnumerable<object[]> SelectTestCases
        {
            get
            {
                yield return new object[]
                {
                    Select(12.July(2010).Literal()), new SqliteRendererSettings { PrettyPrint = false, DateFormatString = "dd/MM/yyyy" },
                    $"SELECT '{12.July(2010).ToString("dd/MM/yyyy")}'"
                };

                yield return new object[]
                {
                    Select(1.Literal()), new SqliteRendererSettings { PrettyPrint = false },
                    "SELECT 1"
                };

                yield return new object[]
                {
                    Select(1L.Literal()), new SqliteRendererSettings { PrettyPrint = false },
                    "SELECT 1"
                };

                yield return new object[]
                {
                    Select(Null("TextValue".Field(), "IntegerValue".Field(), "RealValue".Field())),
                    new SqliteRendererSettings { PrettyPrint = false},
                    @"SELECT COALESCE(""TextValue"", ""IntegerValue"", ""RealValue"")"
                };

                yield return new object[]
                {
                    Select(1.Literal()).Union(Select(2.Literal())), new SqliteRendererSettings { PrettyPrint = false },
                    "SELECT 1 UNION SELECT 2"
                };

                yield return new object[]
                {
                    Select(1.Literal()).Union(Select(2.Literal())), new SqliteRendererSettings { PrettyPrint = true },
                    $"SELECT 1 {Environment.NewLine}UNION {Environment.NewLine}SELECT 2"
                };

                yield return new object[]
                {
                    Select("fullname")
                    .From(
                        Select(Concat("firstname".Field(), " ".Literal(),  "lastname".Field()).As("fullname"))
                        .From("people").As("p")
                    ),
                    new SqliteRendererSettings { PrettyPrint = false },
                    @"SELECT ""fullname"" FROM (SELECT ""firstname"" || ' ' || ""lastname"" AS ""fullname"" FROM ""people"") ""p"""
                };

                yield return new object[]
                {
                    Select("firstname".Field(), "lastname".Field())
                        .From("people")
                        .Where("firstname".Field().IsNotNull()),
                    new SqliteRendererSettings { PrettyPrint = false },
                    @"SELECT ""firstname"", ""lastname"" FROM ""people"" WHERE (""firstname"" IS NOT NULL)"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("Fullname"))
                    .From("superheroes")
                    .Where("nickname".Field(), EqualTo, "Batman"),
                    new SqliteRendererSettings { PrettyPrint = false },
                    "BEGIN;" +
                    "PRAGMA temp_store = 2;" +
                    @"CREATE TEMP TABLE ""_VARIABLES""(ParameterName TEXT PRIMARY KEY, RealValue REAL, IntegerValue INTEGER, BlobValue BLOB, TextValue TEXT);" +
                    @"INSERT INTO ""_VARIABLES"" (""ParameterName"") VALUES ('p0');" +
                    @"UPDATE ""_VARIABLES"" SET ""TextValue"" = 'Batman' WHERE (""ParameterName"" = 'p0');" +
                    @"SELECT ""firstname"" || ' ' || ""lastname"" AS ""Fullname"" " +
                    @"FROM ""superheroes"" " +
                    @"WHERE (""nickname"" = (SELECT COALESCE(""RealValue"", ""IntegerValue"", ""BlobValue"", ""TextValue"") FROM ""_VARIABLES"" WHERE (""ParameterName"" = 'p0') LIMIT 1));" +
                    @"DROP TABLE ""_VARIABLES"";" +
                    "END;"
                };
            }
        }

        [Theory]
        [MemberData(nameof(SelectTestCases))]
        public void SelectTest(SelectQuery query, SqliteRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        private void IsQueryOk(IQuery query, SqliteRendererSettings settings, string expectedString)
        {
            _outputHelper.WriteLine($"{nameof(query)} : {query}");
            _outputHelper.WriteLine($"{nameof(settings)} : {settings}");
            // Act
            string result = query.ForSqlite(settings);

            // Assert
            result.Should().Be(expectedString);
        }
    }
}
