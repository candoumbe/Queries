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

        public SqlServerRendererTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public void Dispose()
        {
            _outputHelper = null;
        }

        //private class Cases
        //{
        //    public IEnumerable<TestCaseData> SelectTestCases
        //    {
        //        get
        //        {
        //            yield return new TestCaseData(Select("*").From("Table"))
        //                .SetName($"{nameof(Select)}(""*"").{nameof(SelectQuery.From)}(""Table"")")
        //                .Returns("SELECT * FROM [Table]");

        //            yield return new TestCaseData(Select("Employees.*").From("Table"))
        //                .SetName($"{nameof(Select)}(""Employees.*"").{nameof(SelectQuery.From)}(""Table"")")
        //                .Returns("SELECT [Employees].* FROM [Table]");

        //            yield return new TestCaseData(Select("*").From("Table".Table("t")))
        //                .SetName($"{nameof(Select)}(""*"").{nameof(SelectQuery.From)}(""Table"".{nameof(StringExtensions.Table)}(""t""))")
        //                .Returns("SELECT * FROM [Table] [t]");



        //            yield return new TestCaseData(Select("Col1".Literal()).From("Table".Table("t")))
        //                .SetName($"{nameof(Select)}(""Col1"".{nameof(LiteralExtensions.Literal)}()).{nameof(SelectQuery.From)}(""Table"".{nameof(StringExtensions.Table)}(""t""))")
        //                .Returns("SELECT 'Col1' FROM [Table] [t]");

        //            yield return new TestCaseData(Select("Col1".Field()).From("Table".Table("t")))
        //                .SetName($"{nameof(Select)}(""Col1"".{nameof(LiteralExtensions.Literal)}()).{nameof(SelectQuery.From)}(""Table"".{nameof(StringExtensions.Table)}(""t""))")
        //                .Returns("SELECT [Col1] FROM [Table] [t]");


        //            yield return new TestCaseData(
        //                Select("Col1".Field())
        //                .From("Table1".Table("t1"))
        //                .Union(
        //                    Select("Col2")
        //                    .From("Table2".Table("t2")))
        //                )
        //                .SetName($"{nameof(Select)}(""Col1"".{nameof(StringExtensions.Field)}()).{nameof(SelectQuery.From)}(""Table"".{nameof(StringExtensions.Table)}(""t1"")).{nameof(Core.Builders.Fluent.IUnionQuery<SelectQuery>.Union)}({nameof(Select)}(""Col2"").{nameof(SelectQuery.From)}(""Table"".{nameof(StringExtensions.Table)}(""t2"")))")
        //                .Returns("SELECT [Col1] FROM [Table1] [t1] UNION SELECT [Col2] FROM [Table2] [t2]");

        //            yield return new TestCaseData(Select(1.Literal()).Union(Select(2.Literal())))
        //               .SetName($"{nameof(Select)}(1.{nameof(LiteralExtensions.Literal)}()).{nameof(Core.Builders.Fluent.IUnionQuery<SelectQuery>.Union)}({nameof(Select)}(2.{nameof(LiteralExtensions.Literal)}())")
        //               .Returns("SELECT 1 UNION SELECT 2");


        //            yield return new TestCaseData(
        //                Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname"))
        //                .From("members")
        //                )
        //                .SetName($"{nameof(Select)}({nameof(Concat)}([firstname].{nameof(StringExtensions.Field)}(), "" "".{nameof(LiteralExtensions.Literal)}(), [lastname].{nameof(StringExtensions.Field)}()).{nameof(SelectQuery.From)}([members])")
        //                .Returns("SELECT [firstname] + ' ' + [lastname] AS [fullname] FROM [members]");


        //        }
        //    }


        //    public IEnumerable<TestCaseData> DeleteTestCases
        //    {
        //        get
        //        {
        //            yield return new TestCaseData(Delete("members"))
        //                .Returns("DELETE FROM [members]")
        //                .SetName($"{nameof(Delete)}([members])");

        //            yield return new TestCaseData(Delete("members").Where("firstname".Field(), NotEqualTo, "Khal El"))
        //                .Returns("DELETE FROM [members] WHERE ([firstname] <> 'Khal El')")
        //                .SetName($"{nameof(Delete)}([members]).{nameof(DeleteQuery.Where)}([firstname].{nameof(StringExtensions.Field)}(), {nameof(NotEqualTo)}, ""Khal El"")");
        //        }
        //    }
        //}


        //[TestCaseSource(typeof(Cases), nameof(Cases.SelectTestCases))]
        //public string SelectTestCases(IQuery query) => query.ForSqlServer();

        //[TestCaseSource(typeof(Cases), nameof(Cases.DeleteTestCases))]
        //public string DeleteTestCases(IQuery query) => query.ForSqlServer();

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
            }
        }

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
        [MemberData(nameof(SelectTestCases))]
        public void SelectTest(SelectQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);

        [Theory]
        [MemberData(nameof(UpdateTestCases))]
        public void UpdateTest(UpdateQuery query, bool prettyPrint, string expectedString)
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
