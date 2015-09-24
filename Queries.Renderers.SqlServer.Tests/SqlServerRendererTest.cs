using System;
using System.Collections.Generic;
using NUnit.Framework;
using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Extensions;
using Queries.Core.Parts.Clauses;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using static Queries.Core.Parts.Clauses.ClauseOperator;

namespace Queries.Renderers.SqlServer.Tests
{
    
    public class SqlServerRendererTest
    {


        private class Cases
        {
            public IEnumerable<TestCaseData> SelectTestCases
            {
                get
                {
                    yield return new TestCaseData(Select("*").From("Table"))
                        .SetName($@"{nameof(Select)}(""*"").{nameof(SelectQuery.From)}(""Table"")")
                        .Returns("SELECT * FROM [Table]");

                    yield return new TestCaseData(Select("Employees.*").From("Table"))
                        .SetName($@"{nameof(Select)}(""Employees.*"").{nameof(SelectQuery.From)}(""Table"")")
                        .Returns("SELECT [Employees].* FROM [Table]");

                    yield return new TestCaseData(Select("*").From("Table".Table("t")))
                        .SetName($@"{nameof(Select)}(""*"").{nameof(SelectQuery.From)}(""Table"".{nameof(StringExtensions.Table)}(""t""))")
                        .Returns("SELECT * FROM [Table] [t]");



                    yield return new TestCaseData(Select("Col1".Literal()).From("Table".Table("t")))
                        .SetName($@"{nameof(Select)}(""Col1"".{nameof(LiteralExtensions.Literal)}()).{nameof(SelectQuery.From)}(""Table"".{nameof(StringExtensions.Table)}(""t""))")
                        .Returns("SELECT 'Col1' FROM [Table] [t]");

                    yield return new TestCaseData(Select("Col1".Field()).From("Table".Table("t")))
                        .SetName($@"{nameof(Select)}(""Col1"".{nameof(LiteralExtensions.Literal)}()).{nameof(SelectQuery.From)}(""Table"".{nameof(StringExtensions.Table)}(""t""))")
                        .Returns("SELECT [Col1] FROM [Table] [t]");


                    yield return new TestCaseData(
                        Select("Col1".Field())
                        .From("Table1".Table("t1"))
                        .Union(
                            Select("Col2")
                            .From("Table2".Table("t2")))
                        )
                        .SetName($@"{nameof(Select)}(""Col1"".{nameof(StringExtensions.Field)}()).{nameof(SelectQuery.From)}(""Table"".{nameof(StringExtensions.Table)}(""t1"")).{nameof(Core.Builders.Fluent.IUnionQuery<SelectQuery>.Union)}({nameof(Select)}(""Col2"").{nameof(SelectQuery.From)}(""Table"".{nameof(StringExtensions.Table)}(""t2"")))")
                        .Returns("SELECT [Col1] FROM [Table1] [t1] UNION SELECT [Col2] FROM [Table2] [t2]");

                    yield return new TestCaseData(Select(1.Literal()).Union(Select(2.Literal())))
                       .SetName($@"{nameof(Select)}(1.{nameof(LiteralExtensions.Literal)}()).{nameof(Core.Builders.Fluent.IUnionQuery<SelectQuery>.Union)}({nameof(Select)}(2.{nameof(LiteralExtensions.Literal)}())")
                       .Returns("SELECT 1 UNION SELECT 2");


                    yield return new TestCaseData(
                        Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname"))
                        .From("members")
                        )
                        .SetName($@"{nameof(Select)}({nameof(Concat)}(""firstname"".{nameof(StringExtensions.Field)}(), "" "".{nameof(LiteralExtensions.Literal)}(), ""lastname"".{nameof(StringExtensions.Field)}()).{nameof(SelectQuery.From)}(""members"")")
                        .Returns("SELECT [firstname] + ' ' + [lastname] AS [fullname] FROM [members]");


                }
            }


            public IEnumerable<TestCaseData> DeleteTestCases
            {
                get
                {
                    yield return new TestCaseData(Delete("members"))
                        .Returns("DELETE FROM [members]")
                        .SetName($@"{nameof(Delete)}(""members"")");

                    yield return new TestCaseData(Delete("members").Where("firstname".Field(), NotEqualTo, "Khal El"))
                        .Returns("DELETE FROM [members] WHERE ([firstname] <> 'Khal El')")
                        .SetName($@"{nameof(Delete)}(""members"").{nameof(DeleteQuery.Where)}(""firstname"".{nameof(StringExtensions.Field)}(), {nameof(NotEqualTo)}, ""Khal El"")");
                }
            }
        }


        [TestCaseSource(typeof(Cases), nameof(Cases.SelectTestCases))]
        public string SelectTestCases(IQuery query) => query.ForSqlServer();

        [TestCaseSource(typeof(Cases), nameof(Cases.DeleteTestCases))]
        public string DeleteTestCases(IQuery query) => query.ForSqlServer();
    }
}
