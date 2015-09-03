using System;
using System.Collections.Generic;
using NUnit.Framework;
using Queries.Builders;
using Queries.Parts;
using Queries.Parts.Columns;
using Queries.Renderers;

namespace Queries.Tests.Renderers
{
    public class PostgresRendererTest
    {

        private ISqlRenderer _renderer;

        private class Cases
        {
            public IEnumerable<ITestCaseData> SelectTestsCases
            {
                get
                {
                    yield return new TestCaseData(null)
                        .SetName("Select null query")
                        .Returns(String.Empty);

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){Name = "Col1"}
                        },
                        From = new[]
                        {
                            new TableTerm(){Name = "Table", Alias = "t"}
                        },

                    })
                        .SetName("Select with one Table column and one table in the FROM part")
                        .Returns("SELECT \"Col1\" FROM \"Table\" \"t\"");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){Name = "Col1"}
                        },
                        From = new[]
                        {
                            new TableTerm(){Name = "dbo.Table", Alias = "t"}
                        },

                    })
                        .SetName("Select with one Table column and one table in the FROM part and by specifying a table owner")
                        .Returns("SELECT \"Col1\" FROM \"dbo\".\"Table\" \"t\"");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){Name = "Col1"},
                            new TableColumn(){Name = "Col2"}
                        },
                        From = new[]
                        {
                            new TableTerm(){Name = "Table1", Alias = "t1"},
                            new TableTerm(){Name = "Table2", Alias = "t2"}
                        },

                    })
                        .SetName("Select with two columns and two tables in the FROM part")
                        .Returns("SELECT \"Col1\", \"Col2\" FROM \"Table1\" \"t1\", \"Table2\" \"t2\"");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){Name = "Col1", Alias = "Alias"}
                        },
                        From = new[]
                        {
                            new TableTerm(){Name = "Table", Alias = "t"}
                        },

                    })
                        .SetName("Select with one Table column that has an alias")
                        .Returns("SELECT \"Col1\" \"Alias\" FROM \"Table\" \"t\"");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){Name = "Col1"},
                            new TableColumn(){Name = "Col2"},
                        },
                        From = new[]
                        {
                            new TableTerm(){Name = "Table", Alias = "t"}
                        },

                    })
                        .SetName("Select with two table columns")
                        .Returns("SELECT \"Col1\", \"Col2\" FROM \"Table\" \"t\"");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){Name = "Col2"},
                            new TableColumn(){Name = "Col1"},
                            
                        },
                        From = new[]
                        {
                            new TableTerm(){Name = "Table", Alias = "t"}
                        },

                    })
                        .SetName("Select with two table columns col2 before col1")
                        .SetDescription("The renderer must respect the ")
                        .Returns("SELECT \"Col2\", \"Col1\" FROM \"Table\" \"t\"");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new TableColumn(){Name = "Col1"})
                        },
                        From = new[]
                        {
                            new TableTerm(){Name = "Table", Alias = "t"}
                        },

                    })
                        .SetName("Select with one aggregate column MIN")
                        .Returns("SELECT MIN(\"Col1\") FROM \"Table\" \"t\"");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new TableColumn(){Name = "Col1", Alias = "Alias"})
                        },
                        From = new[]
                        {
                            new TableTerm(){Name = "Table", Alias = "t"}
                        },

                    })
                        .SetName("Select with one aggregate column MIN with an alias")
                        .Returns("SELECT MIN(\"Col1\") \"Alias\" FROM \"Table\" \"t\"");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new TableColumn(){Name = "Col1", Alias = "Minimum"}),
                            new Max(new TableColumn(){Name = "Col2", Alias = "Maximum"})
                        },
                        From = new[]
                        {
                            new TableTerm(){Name = "Table", Alias = "t"}
                        },

                    })
                        .SetName("Select with two aggregate column MIN with an alias")
                        .Returns("SELECT MIN(\"Col1\") \"Minimum\", MAX(\"Col2\") \"Maximum\" FROM \"Table\" \"t\"");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new TableColumn(){Name = "Col1", Alias = "Minimum"}),
                            new TableColumn(){Name = "Col2", Alias = "Maximum"}
                        },
                        From = new[]
                        {
                            new TableTerm(){Name = "Table", Alias = "t"}
                        },

                    })
                        .SetName("Select with one aggregate column MIN and one table column")
                        .SetDescription("This should generate a GROUP BY clause")
                        .Returns("SELECT MIN(\"Col1\") \"Minimum\", \"Col2\" \"Maximum\" FROM \"Table\" \"t\" GROUP BY \"Col2\"");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new TableColumn(){Name = "Col1", Alias = "Minimum"}),
                        },
                        From = new[]
                        {
                            new TableTerm(){Name = "Table", Alias = "t"}
                        },
                        Union = new[]{
                            new SelectQuery()
                            { 
                                Select = new IColumn[]
                                {
                                    new Min(new TableColumn(){Name = "Col2", Alias = "Minimum"}),
                                },
                                From = new[]
                                {
                                    new TableTerm(){Name = "Table2", Alias = "t2"}
                                }
                            }
                        }
                    })
                        .SetName("Select with one union")
                        .Returns("SELECT MIN(\"Col1\") \"Minimum\" FROM \"Table\" \"t\" UNION SELECT MIN(\"Col2\") \"Minimum\" FROM \"Table2\" \"t2\"");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){ Name = "civ_prenom"}
                        },

                        From = new[]
                        {
                            new TableTerm(){ Name =  "t_civilite", Alias = "civ"}
                        },
                        Where = new WhereClause()
                        {
                            Column = new TableColumn() { Name = "civ_nom" },
                            Operator = WhereOperator.EqualTo,
                            Constraint = new LiteralColumn("dupont")
                        }
                    })
                        .SetName("SELECT with one WHERE clause")
                        .Returns("SELECT \"civ_prenom\" FROM \"t_civilite\" \"civ\" WHERE (\"civ_nom\" = 'dupont')");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){ Name = "prenom"}
                        },

                        From = new[]
                        {
                            new TableTerm(){ Name =  "t_person", Alias = "p"}
                        },
                        Where = new CompositeWhereClause()
                        {
                            Logic = WhereLogic.Or,
                            Clauses = new List<IClause>()
                            {
                                new WhereClause() {Column = new TableColumn(){ Name = "per_age"}, Operator = WhereOperator.GreaterThanOrEqualTo, Constraint = 15},
                                new WhereClause() {Column = new TableColumn(){ Name = "per_age"}, Operator = WhereOperator.LessThan, Constraint = 18}
                            }
                        }
                    })
                        .SetName("SELECT with composite WHERE clause")
                        .Returns("SELECT \"prenom\" FROM \"t_person\" \"p\" WHERE ((\"per_age\" >= 15) OR (\"per_age\" < 18))");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){ Name = "prenom"}
                        },

                        From = new[]
                        {
                            new TableTerm(){ Name =  "t_person", Alias = "p"}
                        },
                        Where = new CompositeWhereClause()
                        {
                            Logic = WhereLogic.And,
                            Clauses = new IClause[]
                            {
                                new CompositeWhereClause()
                                {
                                    Logic = WhereLogic.Or,
                                    Clauses = new IClause[]{
                                        new WhereClause(){Column = new TableColumn{Name = "per_nom"}, Operator = WhereOperator.EqualTo, Constraint = "dupont"},
                                        new WhereClause(){Column = new TableColumn{Name = "per_nom"}, Operator = WhereOperator.EqualTo, Constraint = "durant"},
                                
                                    }
                                },

                                new CompositeWhereClause()
                                {
                                    Logic = WhereLogic.Or,
                                    Clauses = new IClause[]{
                                        new WhereClause(){Column = new TableColumn{Name = "per_age"}, Operator = WhereOperator.GreaterThanOrEqualTo, Constraint = 15},
                                        new WhereClause(){Column = new TableColumn{Name = "per_age"}, Operator = WhereOperator.LessThan, Constraint = 18},
                                
                                    }
                                }
                            }
                        }
                    })
                        .SetName("SELECT with Composite clause + composite clause")
                        .Returns("SELECT \"prenom\" FROM \"t_person\" \"p\" WHERE (((\"per_nom\" = 'dupont') OR (\"per_nom\" = 'durant')) AND ((\"per_age\" >= 15) OR (\"per_age\" < 18)))");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){ Name = "prenom"}
                        },

                        From = new[]
                        {
                            new TableTerm(){ Name =  "t_person", Alias = "p"}
                        },
                        Where = new CompositeWhereClause()
                        {
                            Logic = WhereLogic.And,
                            Clauses = new IClause[]
                            {
                                new WhereClause(){Column = new TableColumn{Name = "per_nom"}, Operator = WhereOperator.EqualTo, Constraint = "dupont"},
                                new CompositeWhereClause()
                                {
                                    Logic = WhereLogic.Or,
                                    Clauses = new IClause[]{
                                        new WhereClause(){Column = new TableColumn{Name = "per_age"}, Operator = WhereOperator.GreaterThanOrEqualTo, Constraint = 15},
                                        new WhereClause(){Column = new TableColumn{Name = "per_age"}, Operator = WhereOperator.LessThan, Constraint = 18},
                                
                                    }
                                }
                            }
                        }
                    })
                        .SetName("SELECT with WhereClause + Composite clause")
                        .Returns("SELECT \"prenom\" FROM \"t_person\" \"p\" WHERE ((\"per_nom\" = 'dupont') AND ((\"per_age\" >= 15) OR (\"per_age\" < 18)))");



                }
            }
        }

        [SetUp]
        public void SetUp()
        {
            _renderer = new PostgresqlRenderer();
        }

        [TearDown]
        public void TearDown()
        {
            _renderer = null;
        }


        [TestCaseSource(typeof(Cases), "SelectTestsCases")]
        public string RenderSelect(SelectQuery selectQuery)
        {
            return _renderer.Render(selectQuery);
        }
    }
}