using System;
using System.Collections.Generic;
using NUnit.Framework;
using Queries.Builders;
using Queries.Builders.Fluent;
using Queries.Parts;
using Queries.Parts.Columns;
using Queries.Renderers;

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

#region SQL SERVER

                    yield return
                        new TestCaseData(new Select("col1", "col2").From("table1").Build(), DatabaseType.SqlServer)
                            .SetName(@"""new Select(""col1"", ""col2"").From(""table1"")""")
                            .SetCategory("SQL SERVER")
                            .Returns("SELECT [col1], [col2] FROM [table1]");
                    yield return
                        new TestCaseData(
                            new Select(new Concat(TableColumn.From("firstname"), LiteralColumn.From(" "),
                                TableColumn.From("lastname")) {Alias = "fullname"})
                                .From("table1").Build(), DatabaseType.SqlServer)
                            .SetName(
                                @"""new Select(new Concat(""fullname"", TableColumn.From(""firstname""), LiteralColumn.From(""""), TableColumn.From(""lastname"")).From(""table1"").Build()")
                            .SetCategory("SQL SERVER")
                            .Returns(@"SELECT [firstname] + ' ' + [lastname] AS [fullname] FROM [table1]");

                    yield return new TestCaseData(new Select(new Concat(TableColumn.From("firstname"), LiteralColumn.From(" "), TableColumn.From("lastname")){Alias = "fullname"})
                            .From("table1")
                            .Where(new CompositeWhereClause()
                            {
                                Logic = WhereLogic.And,
                                Clauses = new IClause[]
                                {
                                    new WhereClause(){Column = TableColumn.From("age"), Operator = WhereOperator.GreaterThanOrEqualTo, Constraint = 15},
                                    new WhereClause(){Column = TableColumn.From("age"), Operator = WhereOperator.LessThan, Constraint = 18}
                                }
                            })
                            .Build(),
                            DatabaseType.SqlServer)
                        .SetName(@"""new Select(new Concat(""fullname"", TableColumn.From(""firstname""), LiteralColumn.From(""""), TableColumn.From(""lastname"")).From(""table1"").Build()")
                        .SetCategory("SQL SERVER")
                        .Returns(@"SELECT [firstname] + ' ' + [lastname] AS [fullname] FROM [table1] WHERE (([age] >= 15) AND ([age] < 18))");
#endregion



                    #region POSTGRES
                    yield return new TestCaseData(new Select("col1", "col2").From("table1").Build(), DatabaseType.Postgres)
                                    .SetName(@"""new Select(""col1"", ""col2"").From(""table1"")""")
                                    .SetCategory("POSTGRES")
                                    .Returns(@"SELECT ""col1"", ""col2"" FROM ""table1""");

                    yield return new TestCaseData(new Select(new Concat(TableColumn.From("firstname"), LiteralColumn.From(" "), TableColumn.From("lastname")) { Alias = "fullname" })
                            .From("table1").Build(), DatabaseType.Postgres)
                        .SetName(@"""new SelectQueryBuilder()..Select(new Concat(""fullname"", TableColumn.From(""firstname""), LiteralColumn.From(""""), TableColumn.From(""lastname"")).From(""table1"").Build()")
                        .SetCategory("POSTGRES")
                        .Returns(@"SELECT ""firstname"" || ' ' || ""lastname"" ""fullname"" FROM ""table1"""); 
                    #endregion

                   
                }
            }
        }

        [TestCaseSource(typeof(Cases), "SelectQueryBuilderTestCases")]
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
