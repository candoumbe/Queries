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

namespace Queries.Renderers.SqlServer.Tests
{

    public class SqlServerRendererTest : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public SqlServerRendererTest(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public void Dispose() => _outputHelper = null;


        public static IEnumerable<object[]> SelectTestCases
        {
            get
            {
                yield return new object[] { Select(UUID()), false, "SELECT NEWID()" };

                yield return new object[] { Select(1.Literal()), false, "SELECT 1" };

                yield return new object[] { Select(1L.Literal()), false, "SELECT 1" };

                yield return new object[] { Select(1.Literal()).Union(Select(2.Literal())), false, "SELECT 1 UNION SELECT 2" };

                yield return new object[] { Select(1.Literal()).Union(Select(2.Literal())), true, $"SELECT 1 {Environment.NewLine}UNION {Environment.NewLine}SELECT 2" };

                yield return new object[]
                {
                    Select("fullname")
                    .From(
                        Select(Concat("firstname".Field(), " ".Literal(),  "lastname".Field()).As("fullname"))
                        .From("people").As("p")),
                    false,
                    "SELECT [fullname] FROM (SELECT [firstname] + ' ' + [lastname] AS [fullname] FROM [people]) [p]"

                };


                yield return new object[]
                {
                    Select("firstname".Field(), "lastname".Field())
                        .From("people")
                        .Where("firstname".Field().IsNotNull()),
                    false,
                    "SELECT [firstname], [lastname] FROM [people] WHERE ([firstname] IS NOT NULL)"

                };

                yield return new object[]
                {
                    Select(1.2f.Literal()),
                    false,
                    $"SELECT {1.2f}"

                };

                yield return new object[]
                {
                    Select(double.MaxValue.Literal()),
                    false,
                    $"SELECT {double.MaxValue}"

                };

                yield return new object[]
                {
                    Select("*").From(Select(1.Literal()).Union(Select(2.Literal()))), false, "SELECT * FROM (SELECT 1 UNION SELECT 2)"
                };

                yield return new object[] {
                    Select("*")
                    .From(
                        Select("identifier").From("identities").Union(Select("username").From("members")).As("logins")
                        ), false,
                    "SELECT * FROM (SELECT [identifier] FROM [identities] UNION SELECT [username] FROM [members]) [logins]"
                };


                yield return new object[]
                {
                    Select("*").From("Table"), false,
                    "SELECT * FROM [Table]"
                };

                yield return new object[] { Select("*".Field()).From("Table"), false, "SELECT * FROM [Table]" };

                yield return new object[] { Select("Employees.*").From("Table"), false, "SELECT [Employees].* FROM [Table]" };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field())).From("members"), false,
                    "SELECT [firstname] + ' ' + [lastname] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field())).From("members"), false,
                    "SELECT [firstname] + ' ' + [lastname] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    , false,
                    "SELECT [firstname] + ' ' + [lastname] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy(new SortExpression("firstname"))
                    , false,
                    "SELECT [firstname] + ' ' + [lastname] FROM [members] ORDER BY [firstname]"
                };



                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy(new SortExpression("firstname", SortDirection.Descending))
                    , false,
                    "SELECT [firstname] + ' ' + [lastname] FROM [members] ORDER BY [firstname] DESC"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    , false,
                    "SELECT [firstname] + ' ' + [lastname] FROM [members] ORDER BY [firstname] DESC"
                };

                yield return new object[]
                {
                    Select(Length(Concat("firstname".Field(), " ".Literal(), "lastname".Field())))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    , false,
                    "SELECT LEN([firstname] + ' ' + [lastname]) FROM [members] ORDER BY [firstname] DESC"
                };

                yield return new object[]
                {
                    Select(Length(Min(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    , false,
                    "SELECT LEN(MIN([firstname] + ' ' + [lastname])) FROM [members] ORDER BY [firstname] DESC"
                };

                yield return new object[]
                {
                    Select(Min(Length(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    , false,
                    "SELECT MIN(LEN([firstname] + ' ' + [lastname])) FROM [members] ORDER BY [firstname] DESC"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname")).From("members"), false,
                    "SELECT [firstname] + ' ' + [lastname] AS [fullname] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Null("firstname".Field(), "").As("firstname")).From("members"), false,
                    "SELECT ISNULL([firstname], '') AS [firstname] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Max("age".Field()).As("age maxi")).From("members"), false,
                    "SELECT MAX([age]) AS [age maxi] FROM [members]"
                };


                yield return new object[]
                {
                    Select(Max(Null("age".Field(), 0)).As("age maxi")).From("members"), false,
                    "SELECT MAX(ISNULL([age], 0)) AS [age maxi] FROM [members]"
                };


                yield return new object[]
                {
                    Select(Min("age".Field()).As("age mini")).From("members"), false,
                    "SELECT MIN([age]) AS [age mini] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Min(Null("age".Field(), 0)).As("age mini")).From("members"), false,
                    "SELECT MIN(ISNULL([age], 0)) AS [age mini] FROM [members]"
                };

                yield return new object[]
                {
                    Select("firstname".Field(), Max("age".Field()).As("age maximum"))
                    .From("members"),
                    false,
                    "SELECT [firstname], MAX([age]) AS [age maximum] FROM [members] GROUP BY [firstname]"
                };

                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0, 1)).As("initials"))
                    .From("members"),
                    false,
                    "SELECT SUBSTRING([firstname], 0, 1) + SUBSTRING([lastname], 0, 1) AS [initials] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0)).As("initials"))
                    .From("members"),
                    false,
                    "SELECT SUBSTRING([firstname], 0, 1) + SUBSTRING([lastname], 0) AS [initials] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Substring(Concat("firstname".Field(), "lastname".Field()), 0).As("initials"))
                    .From("members"),
                    false,
                    "SELECT SUBSTRING([firstname] + [lastname], 0) AS [initials] FROM [members]"
                };


                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0)).As("initials"))
                    .From("members"),
                    false,
                    "SELECT SUBSTRING([firstname], 0, 1) + SUBSTRING([lastname], 0) AS [initials] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Upper("firstname".Field())), false,
                    "SELECT UPPER([firstname])"
                };

                {
                    yield return new object[]
                    {
                        Select("Firstname".Field(), "Lastname".Field())
                        .From("SuperHeroes")
                        .Where("Nickname".Field(), EqualTo, "Batman"),
                        false,
                        "DECLARE @nickname AS VARCHAR(8000) = 'Batman';" +
                        "SELECT [Firstname], [Lastname] FROM [SuperHeroes] WHERE ([Nickname] = @nickname)"
                    };
                }

                {
                    string nickname = "Batman";
                    yield return new object[]
                    {
                        Select("Firstname".Field(), "Lastname".Field())
                        .From("SuperHeroes")
                        .Where("Nickname".Field(), EqualTo, $"{nickname}"),
                        true,
                        $"DECLARE @nickname AS VARCHAR(8000) = 'Batman';{Environment.NewLine}" +
                        $"SELECT [Firstname], [Lastname] {Environment.NewLine}" +
                        $"FROM [SuperHeroes] {Environment.NewLine}" +
                        $"WHERE ([Nickname] = @nickname)"
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(SelectTestCases))]
        public void SelectTest(SelectQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);

        public static IEnumerable<object[]> UpdateTestCases
        {
            get
            {
                yield return new object[]
                {
                    Update("members").Set("UUID".Field().EqualTo(UUID())), false,
                    "UPDATE [members] SET [UUID] = NEWID()"
                };
                yield return new object[]
                {
                    Update("members").Set("firstname".Field().EqualTo("")).Where("firstname".Field().IsNull()), false,
                    "UPDATE [members] SET [firstname] = '' WHERE ([firstname] IS NULL)"
                };
                yield return new object[]
                {
                    Update("members").Set("firstname".Field().EqualTo(null)).Where(new WhereClause("firstname".Field(), EqualTo, "")), false,
                    "UPDATE [members] SET [firstname] = NULL WHERE ([firstname] = '')"
                };
            }
        }

        [Theory]
        [MemberData(nameof(UpdateTestCases))]
        public void UpdateTest(UpdateQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);


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
                    false,
                    $"DELETE FROM [members] WHERE ([firstname] IS NULL);{Environment.NewLine}" +
                    $"SELECT * FROM [members]"
                };
            }
        }

        [Theory]
        [MemberData(nameof(BatchQueryCases))]
        public void BatchQueryTest(BatchQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);


        public static IEnumerable<object[]> TruncateQueryCases
        {
            get
            {
                yield return new object[]
                {
                    Truncate("SuperHero"),
                    false,
                    "TRUNCATE TABLE [SuperHero]"
                };
                yield return new object[]
                {
                    Truncate("SuperHero"),
                    true,
                    "TRUNCATE TABLE [SuperHero]"
                };
            }
        }

        [Theory]
        [MemberData(nameof(TruncateQueryCases))]
        public void TruncateQueryTest(TruncateQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);


        public static IEnumerable<object[]> SelectIntoQueryCases
        {
            get
            {
                yield return new object[]
                {
                    SelectInto("SuperHero_BackUp").From(Select("Firstname", "Lastname").From("DCComics")),
                    false,
                    "SELECT * INTO [SuperHero_BackUp] FROM (SELECT [Firstname], [Lastname] FROM [DCComics])"
                };
                yield return new object[]
                {
                    SelectInto("SuperHero_BackUp").From(Select("Firstname", "Lastname").From("DCComics")),
                    true,
                    $"SELECT * {Environment.NewLine}" +
                    $"INTO [SuperHero_BackUp] {Environment.NewLine}" +
                    $"FROM (SELECT [Firstname], [Lastname] {Environment.NewLine}" +
                    $"FROM [DCComics])"
                };
            }
        }

        [Theory]
        [MemberData(nameof(SelectIntoQueryCases))]
        public void SelectIntoQueryTest(SelectIntoQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);



        private void IsQueryOk(IQuery query, bool prettyPrint, string expectedString)
        {
            _outputHelper.WriteLine($"{nameof(query)} : {SerializeObject(query, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })}");
            _outputHelper.WriteLine($"{nameof(prettyPrint)} : {prettyPrint}");
            // Act
            string result = query.ForSqlServer(prettyPrint);

            // Assert
            result.Should().Be(expectedString);
        }
    }
}
