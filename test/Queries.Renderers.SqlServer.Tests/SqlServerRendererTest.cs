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
using Xunit.Abstractions;
using static Newtonsoft.Json.JsonConvert;
using Newtonsoft.Json;
using Queries.Core.Renderers;
using FluentAssertions.Extensions;

namespace Queries.Renderers.SqlServer.Tests
{
    public class SqlServerRendererTest : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public SqlServerRendererTest(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public void Dispose() => _outputHelper = null;

        [Fact]
        public void DefaultConstructor()
        {
            // Act
            SqlServerRenderer renderer = new SqlServerRenderer();

            // Assert
            renderer.Settings.Should().NotBeNull();
            renderer.Settings.PrettyPrint.Should().BeTrue($"{nameof(SqlServerRenderer)}.{nameof(SqlServerRenderer.Settings)}.{nameof(QueryRendererSettings.PrettyPrint)} should be set to true by default");
            renderer.Settings.DateFormatString.Should().Be("yyyy-MM-dd");
        }

        public static IEnumerable<object[]> SelectTestCases
        {
            get
            {
                yield return new object[] { Select(UUID()), new QueryRendererSettings { PrettyPrint = false }, "SELECT NEWID()" };

                yield return new object[] { Select(12.July(2010).Literal()), new QueryRendererSettings { PrettyPrint = false, DateFormatString = "dd/MM/yyyy" }, $"SELECT '{12.July(2010).ToString("dd/MM/yyyy")}'" };

                yield return new object[] { Select(1.Literal()), new QueryRendererSettings { PrettyPrint = false }, "SELECT 1" };

                yield return new object[] { Select(1L.Literal()), new QueryRendererSettings { PrettyPrint = false }, "SELECT 1" };

                yield return new object[] { Select(1.Literal()).Union(Select(2.Literal())), new QueryRendererSettings { PrettyPrint = false }, "SELECT 1 UNION SELECT 2" };

                yield return new object[] { Select(1.Literal()).Union(Select(2.Literal())), new QueryRendererSettings { PrettyPrint = true }, $"SELECT 1 {Environment.NewLine}UNION {Environment.NewLine}SELECT 2" };

                yield return new object[]
                {
                    Select("fullname")
                    .From(
                        Select(Concat("firstname".Field(), " ".Literal(),  "lastname".Field()).As("fullname"))
                        .From("people").As("p")
                    ),
                    new QueryRendererSettings { PrettyPrint = false },
                    "SELECT [fullname] FROM (SELECT [firstname] + ' ' + [lastname] AS [fullname] FROM [people]) [p]"

                };

                yield return new object[]
                {
                    Select("firstname".Field(), "lastname".Field())
                        .From("people")
                        .Where("firstname".Field().IsNotNull()),
                    new QueryRendererSettings { PrettyPrint = false },
                    "SELECT [firstname], [lastname] FROM [people] WHERE ([firstname] IS NOT NULL)"

                };

                yield return new object[]
                {
                    Select("firstname".Field(), "lastname".Field())
                        .From("SuperHero")
                        .Where("Capabilities".Field().NotIn("Super strength", "Heat vision")),
                    new QueryRendererSettings { PrettyPrint = false },
                    "DECLARE @p0 AS VARCHAR(8000) = 'Super strength';" +
                    "DECLARE @p1 AS VARCHAR(8000) = 'Heat vision';" +
                    "SELECT [firstname], [lastname] FROM [SuperHero] WHERE ([Capabilities] NOT IN (@p0, @p1))"

                };

                yield return new object[]
                {
                    Select(1.2f.Literal()),
                    new QueryRendererSettings { PrettyPrint = false },
                    $"SELECT {1.2f}"

                };

                yield return new object[]
                {
                    Select(double.MaxValue.Literal()),
                    new QueryRendererSettings { PrettyPrint = false },
                    $"SELECT {double.MaxValue}"

                };

                yield return new object[]
                {
                    Select("*").From(Select(1.Literal()).Union(Select(2.Literal()))),
                    new QueryRendererSettings { PrettyPrint = false },
                    "SELECT * FROM (SELECT 1 UNION SELECT 2)"
                };

                yield return new object[] {
                    Select("*")
                    .From(
                        Select("identifier").From("identities").Union(Select("username").From("members")).As("logins")
                        ),
                    new QueryRendererSettings { PrettyPrint = false },
                    "SELECT * FROM (SELECT [identifier] FROM [identities] UNION SELECT [username] FROM [members]) [logins]"
                };

                yield return new object[]
                {
                    Select("*").From("Table"),
                    new QueryRendererSettings { PrettyPrint = false },
                    "SELECT * FROM [Table]"
                };

                yield return new object[] {
                    Select("*".Field()).From("Table"),
                    new QueryRendererSettings { PrettyPrint = false },
                    "SELECT * FROM [Table]" };

                yield return new object[] {
                    Select("Employees.*").From("Table"),
                    new QueryRendererSettings { PrettyPrint = false },
                    "SELECT [Employees].* FROM [Table]" };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field())).From("members"),
                    new QueryRendererSettings { PrettyPrint = false },
                    "SELECT [firstname] + ' ' + [lastname] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field())).From("members"),
                    new QueryRendererSettings { PrettyPrint = false },
                    "SELECT [firstname] + ' ' + [lastname] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    , new QueryRendererSettings { PrettyPrint = false },
                    "SELECT [firstname] + ' ' + [lastname] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy(new SortExpression("firstname"))
                    , new QueryRendererSettings { PrettyPrint = false },
                    "SELECT [firstname] + ' ' + [lastname] FROM [members] ORDER BY [firstname]"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy(new SortExpression("firstname", SortDirection.Descending))
                    , new QueryRendererSettings { PrettyPrint = false },
                    "SELECT [firstname] + ' ' + [lastname] FROM [members] ORDER BY [firstname] DESC"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    , new QueryRendererSettings { PrettyPrint = false },
                    "SELECT [firstname] + ' ' + [lastname] FROM [members] ORDER BY [firstname] DESC"
                };

                yield return new object[]
                {
                    Select(Length(Concat("firstname".Field(), " ".Literal(), "lastname".Field())))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    , new QueryRendererSettings { PrettyPrint = false },
                    "SELECT LEN([firstname] + ' ' + [lastname]) FROM [members] ORDER BY [firstname] DESC"
                };

                yield return new object[]
                {
                    Select(Length(Min(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    , new QueryRendererSettings { PrettyPrint = false },
                    "SELECT LEN(MIN([firstname] + ' ' + [lastname])) FROM [members] ORDER BY [firstname] DESC"
                };

                yield return new object[]
                {
                    Select(Min(Length(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    , new QueryRendererSettings { PrettyPrint = false },
                    "SELECT MIN(LEN([firstname] + ' ' + [lastname])) FROM [members] ORDER BY [firstname] DESC"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname")).From("members"),
                    new QueryRendererSettings { PrettyPrint = false },
                    "SELECT [firstname] + ' ' + [lastname] AS [fullname] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Null("firstname".Field(), "").As("firstname")).From("members"),
                    new QueryRendererSettings { PrettyPrint = false },
                    "SELECT ISNULL([firstname], '') AS [firstname] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Max("age".Field()).As("age maxi")).From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "SELECT MAX([age]) AS [age maxi] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Max(Null("age".Field(), 0)).As("age maxi")).From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "SELECT MAX(ISNULL([age], 0)) AS [age maxi] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Min("age".Field()).As("age mini")).From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "SELECT MIN([age]) AS [age mini] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Min(Null("age".Field(), 0)).As("age mini")).From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "SELECT MIN(ISNULL([age], 0)) AS [age mini] FROM [members]"
                };

                yield return new object[]
                {
                    Select("firstname".Field(), Max("age".Field()).As("age maximum"))
                    .From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "SELECT [firstname], MAX([age]) AS [age maximum] FROM [members] GROUP BY [firstname]"
                };

                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0, 1)).As("initials"))
                    .From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "SELECT SUBSTRING([firstname], 0, 1) + SUBSTRING([lastname], 0, 1) AS [initials] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0)).As("initials"))
                    .From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "SELECT SUBSTRING([firstname], 0, 1) + SUBSTRING([lastname], 0) AS [initials] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Substring(Concat("firstname".Field(), "lastname".Field()), 0).As("initials"))
                    .From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "SELECT SUBSTRING([firstname] + [lastname], 0) AS [initials] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0)).As("initials"))
                    .From("members"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "SELECT SUBSTRING([firstname], 0, 1) + SUBSTRING([lastname], 0) AS [initials] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Upper("firstname".Field())),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "SELECT UPPER([firstname])"
                };

                yield return new object[]
                {
                    Select("Firstname".Field(), "Lastname".Field())
                    .From("SuperHeroes")
                    .Where("Nickname".Field(), EqualTo, "Batman"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "DECLARE @p0 AS VARCHAR(8000) = 'Batman';" +
                    "SELECT [Firstname], [Lastname] FROM [SuperHeroes] WHERE ([Nickname] = @p0)"
                };

                yield return new object[]
                {
                    Select("Firstname".Field(), "Lastname".Field())
                    .From("SuperHeroes")
                    .Where("Nickname".Field(), EqualTo, "Batman"),
                    new QueryRendererSettings{ PrettyPrint = true },
                    $"DECLARE @p0 AS VARCHAR(8000) = 'Batman';{Environment.NewLine}" +
                    $"SELECT [Firstname], [Lastname] {Environment.NewLine}" +
                    $"FROM [SuperHeroes] {Environment.NewLine}" +
                    $"WHERE ([Nickname] = @p0)"
                };

                yield return new object[]
                {
                    Select("*")
                    .From(
                        Select("Fullname").From("People").Where("Firstname".Field(), Like, "B%")
                        .Union(
                        Select("Fullname").From("SuperHero").Where("Nickname".Field(), Like, "B%"))
                    ),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "DECLARE @p0 AS VARCHAR(8000) = 'B%';" +
                    "SELECT * " +
                    "FROM (" +
                        "SELECT [Fullname] FROM [People] WHERE ([Firstname] LIKE @p0) " +
                        "UNION " +
                        "SELECT [Fullname] FROM [SuperHero] WHERE ([Nickname] LIKE @p0)" +
                    ")"
                };

                yield return new object[]
                {
                    Select("*").From("members").Where("Firstname".Field(), In, new StringValues("Bruce", "Bane")),
                    new QueryRendererSettings(),
                    "DECLARE @p0 AS VARCHAR(8000) = 'Bruce';" +
                    "DECLARE @p1 AS VARCHAR(8000) = 'Bane';" +
                    "SELECT * FROM [members] WHERE ([Firstname] IN (@p0, @p1))"
                };
            }
        }

        [Theory]
        [MemberData(nameof(SelectTestCases))]
        public void SelectTest(SelectQuery query, QueryRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        public static IEnumerable<object[]> UpdateTestCases
        {
            get
            {
                yield return new object[]
                {
                    Update("members").Set("UUID".Field().EqualTo(UUID())),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "UPDATE [members] SET [UUID] = NEWID()"
                };
                yield return new object[]
                {
                    Update("members").Set("firstname".Field().EqualTo("")).Where("firstname".Field().IsNull()),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "UPDATE [members] SET [firstname] = '' WHERE ([firstname] IS NULL)"
                };
                yield return new object[]
                {
                    Update("members").Set("firstname".Field().EqualTo(null)).Where(new WhereClause("firstname".Field(), EqualTo, "")),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "UPDATE [members] SET [firstname] = NULL WHERE ([firstname] = '')"
                };
            }
        }

        [Theory]
        [MemberData(nameof(UpdateTestCases))]
        public void UpdateTest(UpdateQuery query, QueryRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        public static IEnumerable<object[]> BatchQueryCases
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
                    $"DELETE FROM [members] WHERE ([firstname] IS NULL);{Environment.NewLine}" +
                    $"SELECT * FROM [members]"
                };

                //yield return new object[]
                //{
                //    new BatchQuery(
                //        Declare("p").WithValue("parameterValue").String(),
                //        Delete("members").Where(new WhereClause("firstname".Field(), IsNull)),
                //        Select("*").From("members")
                //    ),
                //    new QueryRendererSettings{ PrettyPrint = false },
                //    $"DECLARE @p VARCHAR(8000) = 'parameterValue';{Environment.NewLine}" +
                //    $"DELETE FROM [members] WHERE ([firstname] IS NULL);{Environment.NewLine}" +
                //    $"SELECT * FROM [members]"
                //};

            }
        }

        [Theory]
        [MemberData(nameof(BatchQueryCases))]
        public void BatchQueryTest(BatchQuery query, QueryRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        public static IEnumerable<object[]> TruncateQueryCases
        {
            get
            {
                yield return new object[]
                {
                    Truncate("SuperHero"),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "TRUNCATE TABLE [SuperHero]"
                };
                yield return new object[]
                {
                    Truncate("SuperHero"),
                    new QueryRendererSettings{ PrettyPrint = true },
                    "TRUNCATE TABLE [SuperHero]"
                };
            }
        }

        [Theory]
        [MemberData(nameof(TruncateQueryCases))]
        public void TruncateQueryTest(TruncateQuery query, QueryRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        public static IEnumerable<object[]> DeleteQueryCases
        {
            get
            {
                yield return new object[]
                {
                    Delete("members")
                    .Where("Activity".Field(), NotLike, "%Super hero%"),
                    new QueryRendererSettings(),
                    "DECLARE @p0 AS VARCHAR(8000) = '%Super hero%';" +
                    "DELETE FROM [members] WHERE ([Activity] NOT LIKE @p0)"
                };
            }
        }

        [Theory]
        [MemberData(nameof(DeleteQueryCases))]
        public void DeleteQueryTests(DeleteQuery query, QueryRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        public static IEnumerable<object[]> SelectIntoQueryCases
        {
            get
            {
                yield return new object[]
                {
                    SelectInto("SuperHero_BackUp").From(Select("Firstname", "Lastname").From("DCComics")),
                    new QueryRendererSettings{ PrettyPrint = false },
                    "SELECT * INTO [SuperHero_BackUp] FROM (SELECT [Firstname], [Lastname] FROM [DCComics])"
                };
                yield return new object[]
                {
                    SelectInto("SuperHero_BackUp").From(Select("Firstname", "Lastname").From("DCComics")),
                    new QueryRendererSettings{ PrettyPrint = true },
                    $"SELECT * {Environment.NewLine}" +
                    $"INTO [SuperHero_BackUp] {Environment.NewLine}" +
                    $"FROM (SELECT [Firstname], [Lastname] {Environment.NewLine}" +
                    $"FROM [DCComics])"
                };
            }
        }

        [Theory]
        [MemberData(nameof(SelectIntoQueryCases))]
        public void SelectIntoQueryTest(SelectIntoQuery query, QueryRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        public static IEnumerable<object[]> SqlInjectionAttackCases
        {
            get
            {
                yield return new object[]
                {
                    Select("id".Field())
                        .From("members")
                        .Where("username".Field(), EqualTo, "Dupont';--"),
                    new QueryRendererSettings(),

                    "DECLARE @p0 AS VARCHAR(8000) = 'Dupont'';--';" +
                    "SELECT [id] FROM [members] WHERE ([username] = @p0)"
                };

                yield return new object[]
                {
                    Select("id".Field())
                        .From("members")
                        .Where("username".Field(), Like, "Du[pont';--"),
                    new QueryRendererSettings(),

                    @"DECLARE @p0 AS VARCHAR(8000) = 'Du\[pont'';--';" +
                    "SELECT [id] FROM [members] WHERE ([username] LIKE @p0)"
                };
            }
        }

        [Theory]
        [MemberData(nameof(SqlInjectionAttackCases))]
        public void PreventSqlInjectionAttack(IQuery query, QueryRendererSettings settings, string expectedString) => IsQueryOk(query, settings, expectedString);

        private void IsQueryOk(IQuery query, QueryRendererSettings settings, string expectedString)
        {
            _outputHelper.WriteLine($"{nameof(query)} : {SerializeObject(query, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })}");
            _outputHelper.WriteLine($"{nameof(settings)} : {SerializeObject(settings)}");
            // Act
            string result = query.ForSqlServer(settings);

            // Assert
            result.Should().Be(expectedString);
        }
    }
}
