using System.Collections.Generic;
using Queries.Core;
using Queries.Core.Builders;
using Xunit;
using static Queries.Core.Builders.Fluent.QueryBuilder;

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

            public IEnumerable<object[]> SelectTestCases
            {
                get
                {
                    yield return new object[]
                    {
                        Select("*").From("Table"),
                        @"SELECT * FROM ""Table"""
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(Cases.SelectTestCases))]
        //[TestCaseSource(typeof(Cases), nameof(Cases.SelectTestCases))]
        public void SelectTestCases(IQuery query, string expectedString) => Assert.Equal(expectedString, query.ForPostgres());


    }
}
