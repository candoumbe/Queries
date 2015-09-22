using System.Collections.Generic;
using NUnit.Framework;
using Queries.Builders;
using Queries.Extensions;
using Queries.Parts.Clauses;
using Queries.Parts.Columns;
using Queries.Renderers;
using static Queries.Builders.Fluent.QueryBuilder;

namespace Queries.Tests.Renderers
{
    public class SelectTest
    {

        private class Cases
        {
            public IEnumerable<ITestCaseData> SelectQueryBuilderTestCases
            {
                get
                {
                    #region SQL Server

                    yield return
                        new TestCaseData(Select("col1", "col2").From("table1").Build(), DatabaseType.SqlServer)
                            .SetName(@"""Select(""col1"", ""col2"").From(""table1"")""")
                            .SetCategory("SQL Server")
                            .Returns("SELECT [col1], [col2] FROM [table1]");
                    yield return
                        new TestCaseData(
                            Select(Concat("fullname", "firstname".Field(), " ".Literal(), "lastname".Field()))
                                .From("table1").Build(), DatabaseType.SqlServer)
                            .SetName(
                                @"""Select(Concat(""fullname"", ""firstname"".Field(), "" "".Literal(), ""lastname"".Field())).From(""table1"").Build()")
                            .SetCategory("SQL Server")
                            .Returns(@"SELECT [firstname] + ' ' + [lastname] AS [fullname] FROM [table1]");

                    yield return new TestCaseData(Select(new Concat("firstname".Field(), " ".Literal(), "lastname".Field()) { Alias = "fullname" })
                            .From("table1")
                            .Where(new CompositeWhereClause()
                            {
                                Logic = ClauseLogic.And,
                                Clauses = new IWhereClause[]
                                {
                                    new WhereClause {Column = "age".Field(), Operator = ClauseOperator.GreaterThanOrEqualTo, Constraint = 15},
                                    new WhereClause {Column = "age".Field(), Operator = ClauseOperator.LessThan, Constraint = 18}
                                }
                            })
                            .Build(),
                            DatabaseType.SqlServer)
                        .SetName(@"""Select(new Concat(""firstname"".Field(), "" "".Literal(), ""lastname"".Field()) {Alias = ""fullname""} ).From(""table1"").Build()")
                        .SetCategory("SQL Server")
                        .Returns(@"SELECT [firstname] + ' ' + [lastname] AS [fullname] FROM [table1] WHERE (([age] >= 15) AND ([age] < 18))");
#endregion

                    #region Postgresql
                    yield return new TestCaseData(Select("col1", "col2").From("table1").Build(), DatabaseType.Postgresql)
                                    .SetName(@"""Select(""col1"", ""col2"").From(""table1"")""")
                                    .SetCategory("Postgresql")
                                    .Returns(@"SELECT ""col1"", ""col2"" FROM ""table1""");

                    yield return new TestCaseData(Select(new Concat("firstname".Field(), " ".Literal(), "lastname".Field()) { Alias = "fullname" })
                            .From("table1").Build(), DatabaseType.Postgresql)
                        .SetName(@"""Select(new Concat(""firstname"".Field(), "" "".Literal(), ""lastname"".Field()) { Alias = ""fullname""}).From(""table1"").Build()")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT ""firstname"" || ' ' || ""lastname"" ""fullname"" FROM ""table1""");


                    yield return new TestCaseData(
                        Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                           .From("table1").Build(), DatabaseType.Postgresql)
                       .SetName(@"""new SelectQueryBuilder()..Select(new Concat(""firstname"".Field(), "" "".Literal(), ""lastname"".Field()) { Alias = ""fullname""}).From(""table1"").Build()")
                       .SetCategory("Postgresql")
                       .Returns(@"SELECT ""firstname"" || ' ' || ""lastname"" ""fullname"" FROM ""table1"""); 
                    #endregion

                   
                }
            }
        }

        [TestCaseSource(typeof(Cases), nameof(Cases.SelectQueryBuilderTestCases))]
        public string Render(SelectQuery selectQuery, DatabaseType databaseType)
        {
            ISqlRenderer renderer;
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                case DatabaseType.SqlServerCompact:
                    renderer = new SqlServerRenderer();
                    break;
                default:
                    renderer = new PostgresqlRenderer();
                    break;
            }

            return renderer.Render(selectQuery);
        }


    }
}
