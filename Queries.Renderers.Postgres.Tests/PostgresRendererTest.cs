using System;
using System.Collections.Generic;
using NUnit.Framework;
using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Extensions;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Renderers.Postgres.Tests
{
    
    public class PostgresRendererTest
    {


        private class Cases
        {
            public IEnumerable<TestCaseData> SelectTestCases
            {
                get
                {
                    yield return new TestCaseData(Select("*").From("Table"))
                        .SetName($@"{nameof(Select)}(""*"").{nameof(SelectQuery.From)}(""Table"")")
                        .Returns(@"SELECT * FROM ""Table""");

                    yield return new TestCaseData(Select("Employees.*").From("Table"))
                        .SetName($@"{nameof(Select)}(""Employees.*"").{nameof(SelectQuery.From)}(""Table"")")
                        .Returns(@"SELECT ""Employees"".* FROM ""Table""");

                    yield return new TestCaseData(Select("*").From("Table".Table("t")))
                        .SetName($@"{nameof(Select)}(""*"").{nameof(SelectQuery.From)}(""Table"".{nameof(StringExtensions.Table)}(""t""))")
                        .Returns(@"SELECT * FROM ""Table"" ""t""");



                }
            }
        }


        [TestCaseSource(typeof(Cases), nameof(Cases.SelectTestCases))]
        public string SelectTestCases(IQuery query) => query.ForPostgres();


    }
}
