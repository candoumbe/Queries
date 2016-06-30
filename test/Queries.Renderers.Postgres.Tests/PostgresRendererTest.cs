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

namespace Queries.Renderers.Postgres.Tests
{
    public class PostgresRendererTest
    {
        private class Cases
        {
            //public IEnumerable<TestCaseData> SelectTestCases
            //{
            //    get
            //    {
            //        yield return new TestCaseData(Select("*").From("Table"))
            //            .SetName($@"{nameof(Select)}(""*"").{nameof(SelectQuery.From)}(""Table"")")
            //            .Returns(@"SELECT * FROM ""Table""");

            //        yield return new TestCaseData(Select("Employees.*").From("Table"))
            //            .SetName($@"{nameof(Select)}(""Employees.*"").{nameof(SelectQuery.From)}(""Table"")")
            //            .Returns(@"SELECT ""Employees"".* FROM ""Table""");

            //        yield return new TestCaseData(Select("*").From("Table".Table("t")))
            //            .SetName($@"{nameof(Select)}(""*"").{nameof(SelectQuery.From)}(""Table"".{nameof(StringExtensions.Table)}(""t""))")
            //            .Returns(@"SELECT * FROM ""Table"" ""t""");



            //    }
            //}
        }
        public static IEnumerable<object[]> SelectTestCases
        {
            get
            {

                yield return new object[] { Select(1.Literal()), false, "SELECT 1"};

                yield return new object[] { Select(1.Literal()).Union(Select(2.Literal())), false, "SELECT 1 UNION SELECT 2" };

                yield return new object[] { Select(1.Literal()).Union(Select(2.Literal())), true, $"SELECT 1 {Environment.NewLine}UNION {Environment.NewLine}SELECT 2" };

                yield return new object[]
                {
                    Select("*").From(Select(1.Literal()).Union(Select(2.Literal()))), false, "SELECT * FROM (SELECT 1 UNION SELECT 2)"
                };

                yield return new object[] {
                    Select("*")
                    .From( 
                        Select("identifier").From("identities").Union(Select("username").From("members")).As("logins") 
                        ), false,
                    @"SELECT * FROM (SELECT ""identifier"" FROM ""identities"" UNION SELECT ""username"" FROM ""members"") ""logins""" };


                yield return new object[]
                {
                    Select("*").From("Table"), false,
                    @"SELECT * FROM ""Table"""
                };

                yield return new object[] {Select("*".Field()).From("Table"), false, @"SELECT * FROM ""Table"""};

                yield return new object[] { Select("Employees.*").From("Table"), false, @"SELECT ""Employees"".* FROM ""Table""" };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field())).From("members"), false,
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field())).From("members"), false,
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    , false,
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy(new SortExpression("firstname"))
                    , false,
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"" ORDER BY ""firstname"""
                };



                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy(new SortExpression("firstname", SortDirection.Descending))
                    , false,
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"" ORDER BY ""firstname"" DESC"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    , false,
                    @"SELECT ""firstname"" || ' ' || ""lastname"" FROM ""members"" ORDER BY ""firstname"" DESC"
                };

                yield return new object[]
                {
                    Select(Length(Concat("firstname".Field(), " ".Literal(), "lastname".Field())))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    , false,
                    @"SELECT LENGTH(""firstname"" || ' ' || ""lastname"") FROM ""members"" ORDER BY ""firstname"" DESC"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname")).From("members"), false,
                    @"SELECT ""firstname"" || ' ' || ""lastname"" ""fullname"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Null("firstname".Field(), "").As("firstname")).From("members"), false,
                    @"SELECT COALESCE(""firstname"", '') ""firstname"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Max("age".Field()).As("age maxi")).From("members"), false,
                    @"SELECT MAX(""age"") ""age maxi"" FROM ""members"""
                };


                yield return new object[]
                {
                    Select(Max(Null("age".Field(), 0)).As("age maxi")).From("members"), false,
                    @"SELECT MAX(COALESCE(""age"", 0)) ""age maxi"" FROM ""members"""
                };


                yield return new object[]
                {
                    Select(Min("age".Field()).As("age mini")).From("members"), false,
                    @"SELECT MIN(""age"") ""age mini"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Min(Null("age".Field(), 0)).As("age mini")).From("members"), false,
                    @"SELECT MIN(COALESCE(""age"", 0)) ""age mini"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select("firstname".Field(), Max("age".Field()).As("age maximum")).From("members"), false,
                    @"SELECT ""firstname"", MAX(""age"") ""age maximum"" FROM ""members"" GROUP BY ""firstname"""
                };


                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0, 1)).As("initials"))
                    .From("members"),
                    false,
                    @"SELECT SUBSTRING(""firstname"" FROM 0 FOR 1) || SUBSTRING(""lastname"" FROM 0 FOR 1) ""initials"" FROM ""members"""
                };

                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0)).As("initials"))
                    .From("members"),
                    false,
                    @"SELECT SUBSTRING(""firstname"" FROM 0 FOR 1) || SUBSTRING(""lastname"" FROM 0) ""initials"" FROM ""members"""
                };


                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0)).As("initials"))
                    .From("members"),
                    false,
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
                    SelectInto("destination").From("source".Table()), false,
                    @"SELECT * INTO ""destination"" FROM ""source"""
                };


                yield return new object[]
                {
                    SelectInto("names")
                        .From(
                            Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname"))
                            .From("members")
                    ),
                    false, 
                    @"SELECT * INTO ""names"" FROM (SELECT ""firstname"" || ' ' || ""lastname"" ""fullname"" FROM ""members"")"

                };

                yield return new object[]
                {
                    SelectInto("names")
                        .From(
                            Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname"))
                                .From("members")
                                .Where("firstname".Field().IsNotNull())),
                    false,
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
                    Update("members").Set("firstname".Field().EqualTo("")).Where("firstname".Field().IsNull()), false,
                    @"UPDATE ""members"" SET ""firstname"" = '' WHERE (""firstname"" IS NULL)"
                };
                yield return new object[]
                {
                     Update("members").Set("firstname".Field().EqualTo(null)).Where(new WhereClause("firstname".Field(), EqualTo, "")), false,
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
                    false,
                    @"DELETE FROM ""members"""
                };

                yield return new object[]
                {
                    Delete("members").Where(new WhereClause("firstname".Field(), IsNull)),
                    false,
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
                    false,
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
                    false,
                    @"INSERT INTO ""members"" SELECT 'Bruce', 'Wayne', 'Batman'"
                };

                yield return new object[]
                {
                    InsertInto("members").Values(Select("Bruce".Literal(), "Wayne".Literal(), "Batman".Literal())),
                    true,
                    $@"INSERT INTO ""members"" {Environment.NewLine}SELECT 'Bruce', 'Wayne', 'Batman'"
                };
                
                yield return new object[]
                {
                    InsertInto("members").Values("firstname".InsertValue("Bruce".Literal()), "lastname".InsertValue("Wayne".Literal()), "nickname".InsertValue("Batman".Literal())),
                    false,
                    @"INSERT INTO ""members"" (""firstname"", ""lastname"", ""nickname"") VALUES ('Bruce', 'Wayne', 'Batman')"
                };

                yield return new object[]
                {
                    InsertInto("members").Values("firstname".InsertValue("Bruce".Literal()), "lastname".InsertValue("Wayne".Literal()), "nickname".InsertValue("Batman".Literal())),
                    true,
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
                    false,
                    $@"DELETE FROM ""members"" WHERE (""firstname"" IS NULL);{Environment.NewLine}SELECT * FROM ""members"""
                };
            }
        }


        [Theory]
        [MemberData(nameof(SelectTestCases))]
        //[TestCaseSource(typeof(Cases), nameof(Cases.SelectTestCases))]
        public void SelectTest(SelectQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);



        [Theory]
        [MemberData(nameof(UpdateTestCases))]
        public void UpdateTest(UpdateQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);

        [Theory]
        [MemberData(nameof(DeleteTestCases))]
        public void DeleteTest(DeleteQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);


        [Theory]
        [MemberData(nameof(SelectIntoTestCases))]
        //[TestCaseSource(typeof(Cases), nameof(Cases.SelectTestCases))]
        public void SelectIntoTest(SelectIntoQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);

        [Theory]
        [MemberData(nameof(TruncateTestCases))]
        //[TestCaseSource(typeof(Cases), nameof(Cases.SelectTestCases))]
        public void TruncateTest(TruncateQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);

        [Theory]
        [MemberData(nameof(InsertIntoTestCases))]
        //[TestCaseSource(typeof(Cases), nameof(Cases.SelectTestCases))]
        public void InsertIntoTest(InsertIntoQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);

        [Theory]
        [MemberData(nameof(BatchTestCases))]
        public void BatchQueryTest(BatchQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);

        private static void IsQueryOk(IQuery query, bool prettyPrint, string expectedString) => Assert.Equal(expectedString, query.ForPostgres(prettyPrint));

    }
}
