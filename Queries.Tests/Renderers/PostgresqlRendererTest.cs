using System;
using System.Collections.Generic;
using NUnit.Framework;
using Queries.Builders;
using Queries.Exceptions;
using Queries.Extensions;
using Queries.Parts;
using Queries.Parts.Clauses;
using Queries.Parts.Columns;
using Queries.Parts.Joins;
using Queries.Parts.Sorting;
using Queries.Renderers;

namespace Queries.Tests.Renderers
{
    public class PostgresqlRendererTest
    {
        private PostgresqlRenderer _renderer;

        private class Cases
        {
            public IEnumerable<ITestCaseData> SelectTestsCases
            {
                get
                {
                    yield return new TestCaseData(null, false)
                        .SetName("Select null query")
                        .SetCategory("Postgresql")
                        .Returns(String.Empty);


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field() {Name = "Col1"}
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, false)
                        .SetName(@"""SELECT <Field{ Name = ""Col1"" }> 
                                FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT ""Col1"" FROM ""Table"" ""t""");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field() {Name = "Col1"}
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, true)
                        .SetName(@"""SELECT <Field{ Name = ""Col1"" }> 
                                FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("Postgresql Pretty Print")
                        .Returns(
                            "SELECT \"Col1\" \r\n" +
                            "FROM \"Table\" \"t\""
                        );

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Concat(
                                new Null(new Field() {Name = "firstname"}, "".Literal()),
                                " ".Literal(),
                                new Null(new Field() {Name = "lastname"}, "".Literal())
                                ) {Alias = "fullname"}
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, false)
                        .SetName(
                            @"""SELECT <Concat(Null(""firstname"".Field }, """".Literal()), "" "".Literal(), Null(""lastname"".Field(), """".Literal())){ Alias = ""fullname"" }> 
                                FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("Postgresql")
                        .Returns(
                            @"SELECT COALESCE(""firstname"", '') || ' ' || COALESCE(""lastname"", '') ""fullname"" FROM ""Table"" ""t""");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, false)
                        .SetName(@"""SELECT FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT * FROM ""Table"" ""t""");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Null(new Field() {Name = "col1"}, "".Literal())
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, false)
                        .SetName(@"""SELECT <Null(Field{ Name = ""col1"" }, """")> 
                                FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT COALESCE(""col1"", '') FROM ""Table"" ""t""");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = null,
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, false)
                        .SetName("Select with no column specified and one table in the FROM part")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT * FROM ""Table"" ""t""");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field() {Name = "Col1"}
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "dbo.Table", Alias = "t"}
                        },

                    }, false)
                        .SetName(
                            @"""SELECT <Field{ Name = ""Col1"" }> FROM <Table { Name = ""dbo.Table"", Alias = ""t"" }>""")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT ""Col1"" FROM ""dbo"".""Table"" ""t""");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field() {Name = "Col1"},
                            new Field() {Name = "Col2"}
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table1", Alias = "t1"},
                            new Table() {Name = "Table2", Alias = "t2"}
                        },

                    }, false)
                        .SetName(
                            @"""SELECT <Field{ Name = ""Col1"" }>, <Field{ Name = ""Col2"" }> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT ""Col1"", ""Col2"" FROM ""Table1"" ""t1"", ""Table2"" ""t2""");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field() {Name = "Col1", Alias = "Alias"}
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        }
                    }, false)
                        .SetName(
                            @"""SELECT <Field{ Name = ""Col1"", Alias = ""Alias"" }> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT ""Col1"" ""Alias"" FROM ""Table"" ""t""");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field() {Name = "Col1"},
                            new Field() {Name = "Col2"},
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, false)
                        .SetName(
                            @"""SELECT <Field{ Name = ""Col1"" }>, <Field{ Name = ""Col2"" }> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT ""Col1"", ""Col2"" FROM ""Table"" ""t""");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new Field() {Name = "Col1"})
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, false)
                        .SetName(
                            @"""SELECT <Min(new Field() {Name = ""Col1""})> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT MIN(""Col1"") FROM ""Table"" ""t""");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min("Col1")
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, false)
                        .SetName(@"""SELECT <new Min (""Col1"")> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT MIN(""Col1"") FROM ""Table"" ""t""");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new Field() {Name = "Col1"})
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, false)
                        .SetName(@"""SELECT <Min(Field {Name = ""Col1""})> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT MIN(""Col1"") FROM ""Table"" ""t""");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new Field() {Name = "Col1", Alias = "Alias"})
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, false)
                        .SetName(@"""SELECT <Min(Field {Name = ""Col1"", Alias = ""Alias""})> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT MIN(""Col1"") FROM ""Table"" ""t""");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new Field() {Name = "Col1"}, "Alias")
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, false)
                        .SetName(@"""SELECT <Min(Field {Name = ""Col1"", Alias = ""Alias""})> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT MIN(""Col1"") ""Alias"" FROM ""Table"" ""t""");



                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new Field() {Name = "Col1"}, "Minimum"),
                            new MaxColumn(new Field() {Name = "Col2"}, "Maximum")
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, false)
                        .SetName(@"""SELECT <Min(Field {Name = ""Col1""}, ""Minimum"")>, <MaxColumn(Field {Name = ""Col2""}, ""Maximum"")> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT MIN(""Col1"") ""Minimum"", MAX(""Col2"") ""Maximum"" FROM ""Table"" ""t""");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new Field() {Name = "Col1"}, "Minimum"),
                            new Field() {Name = "Col2", Alias = "Maximum"}
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, false)
                        .SetName(
                            @"""SELECT <Min(Field {Name = ""Col1"", Alias = ""Minimum""})>, <Field {Name = ""Col2"", Alias = ""Maximum"")> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("Postgresql")
                        .SetDescription("This should generate a GROUP BY clause")
                        .Returns(
                            @"SELECT MIN(""Col1"") ""Minimum"", ""Col2"" ""Maximum"" FROM ""Table"" ""t"" GROUP BY ""Col2""")
                        ;

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new Field() {Name = "Col1"}, "Minimum"),
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },
                        Union = new[]
                        {
                            new SelectQuery()
                            {
                                Select = new IColumn[]
                                {
                                    new Min(new Field() {Name = "Col2", Alias = "Minimum"}),
                                },
                                From = new ITable[]
                                {
                                    new Table() {Name = "Table2", Alias = "t2"}
                                }
                            }
                        }
                    }, false)
                        .SetName(@"""SELECT <Min(Field {Name = ""Col1""}, ""Minimum"")> FROM <Table { Name = ""Table"", Alias = ""t"" }> UNION SELECT <Min(Field {Name = ""Col2"", Alias = ""Minimum""})> FROM <Table { Name = ""Table2"", Alias = ""t2"" }>""")
                        .SetCategory("Postgresql")
                        .Returns(
                            @"SELECT MIN(""Col1"") ""Minimum"" FROM ""Table"" ""t"" UNION SELECT MIN(""Col2"") FROM ""Table2"" ""t2""")
                        ;


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new Field() {Name = "Col1"}, "Minimum"),
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },
                        Union = new[]
                        {
                            new SelectQuery()
                            {
                                Select = new IColumn[]
                                {
                                    new Min(new Field() {Name = "Col2", Alias = "Minimum"}),
                                },
                                From = new ITable[]
                                {
                                    new Table() {Name = "Table2", Alias = "t2"}
                                }
                            }
                        }
                    }, true)
                        .SetName(@"PRETTY PRINT : ""SELECT <Min(Field {Name = ""Col1""}, ""Minimum"")> FROM <Table { Name = ""Table"", Alias = ""t"" }> UNION SELECT <Min(Field {Name = ""Col2"", Alias = ""Minimum""})> FROM <Table { Name = ""Table2"", Alias = ""t2"" }>""")
                        .SetCategory("Postgresql")
                        .Returns(
                            "SELECT MIN(\"Col1\") \"Minimum\" \r\n" +
                            "FROM \"Table\" \"t\" \r\n" +
                            "UNION \r\n" +
                            "SELECT MIN(\"Col2\") \r\n" +
                            @"FROM ""Table2"" ""t2""")
                        ;


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field() {Name = "civ_prenom"}
                        },

                        From = new ITable[]
                        {
                            new Table() {Name = "t_civilite", Alias = "civ"}
                        },
                        Where = new WhereClause()
                        {
                            Column = "civ_nom".Field(),
                            Operator = ClauseOperator.EqualTo,
                            Constraint = "dupont"
                        }
                    }, false)
                        .SetName(
                            @"""SELECT <Field {Name = ""civ_prenom""}> FROM <Table { Name = ""t_civilite"", Alias = ""civ"" }> WHERE <WhereClause { Column = FieldColumn.From(""civ_nom""),Operator = WhereOperator.EqualTo, Constraint = ""dupont""})>")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT ""civ_prenom"" FROM ""t_civilite"" ""civ"" WHERE (""civ_nom"" = 'dupont')");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field() {Name = "civ_prenom"}
                        },

                        From = new ITable[]
                        {
                            new Table() {Name = "t_civilite", Alias = "civ"}
                        },
                        Where = new WhereClause()
                        {
                            Column = "civ_nom".Field(),
                            Operator = ClauseOperator.EqualTo,
                            Constraint = "dupont"
                        }
                    }, true)
                        .SetName(
                            @"""SELECT <Field {Name = ""civ_prenom""}> FROM <Table { Name = ""t_civilite"", Alias = ""civ"" }> WHERE <WhereClause { Column = FieldColumn.From(""civ_nom""),Operator = WhereOperator.EqualTo, Constraint = ""dupont""})>")
                        .SetCategory("Postgresql Pretty Print")
                        .Returns("SELECT \"civ_prenom\" \r\n" +
                                 "FROM \"t_civilite\" \"civ\" \r\n" +
                                 "WHERE (\"civ_nom\" = \'dupont\')");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            "prenom".Field()
                        },
                        From = new ITable[] { "t_person".Table("p") },
                        Where = new CompositeWhereClause()
                        {
                            Logic = ClauseLogic.Or,
                            Clauses = new List<IWhereClause>()
                            {
                                new WhereClause()
                                {
                                    Column = "per_age".Field(),
                                    Operator = ClauseOperator.GreaterThanOrEqualTo,
                                    Constraint = 15
                                },
                                new WhereClause()
                                {
                                    Column = "per_age".Field(),
                                    Operator = ClauseOperator.LessThan,
                                    Constraint = 18
                                }
                            }
                        }
                    }, false)
                        .SetName(
                            @"""SELECT <FieldColumn.From(""prenom"")> FROM <Table { Name = ""t_person"", Alias = ""p"" }> WHERE <CompositeWhereClause {  Logic = WhereLogic.Or, Clauses = new IWhereClause[]{new WhereClause() {Column = new Field(){ Name = ""per_age""}, Operator = WhereOperator.GreaterThanOrEqualTo, Constraint = 15}, new WhereClause(){Column = new Field{Name = ""per_age""}, Operator = WhereOperator.LessThan, Constraint = 18}})")
                        .SetCategory("Postgresql")
                        .Returns(
                            @"SELECT ""prenom"" FROM ""t_person"" ""p"" WHERE ((""per_age"" >= 15) OR (""per_age"" < 18))")
                        ;


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field() {Name = "prenom"}
                        },

                        From = new ITable[]
                        {
                            new Table() {Name = "t_person", Alias = "p"}
                        },
                        Where = new CompositeWhereClause()
                        {
                            Logic = ClauseLogic.And,
                            Clauses = new IWhereClause[]
                            {
                                new CompositeWhereClause()
                                {
                                    Logic = ClauseLogic.Or,
                                    Clauses = new IWhereClause[]
                                    {
                                        new WhereClause()
                                        {
                                            Column = new Field {Name = "per_nom"},
                                            Operator = ClauseOperator.EqualTo,
                                            Constraint = "dupont"
                                        },
                                        new WhereClause()
                                        {
                                            Column = new Field {Name = "per_nom"},
                                            Operator = ClauseOperator.EqualTo,
                                            Constraint = "durant"
                                        }
                                    }
                                },
                                new CompositeWhereClause()
                                {
                                    Logic = ClauseLogic.Or,
                                    Clauses = new IWhereClause[]
                                    {
                                        new WhereClause()
                                        {
                                            Column = new Field {Name = "per_age"},
                                            Operator = ClauseOperator.GreaterThanOrEqualTo,
                                            Constraint = 15
                                        },
                                        new WhereClause()
                                        {
                                            Column = new Field {Name = "per_age"},
                                            Operator = ClauseOperator.LessThan,
                                            Constraint = 18
                                        }
                                    }
                                }
                            }
                        }
                    }, false)
                        .SetName(
                            @"""SELECT <FieldColumn.From(""prenom"")> FROM <Table { Name = ""t_person"", Alias = ""p"" }> WHERE <CompositeWhereClause {  
                        Logic = WhereLogic.And, 
                        Clauses = new IWhereClause[]{
                            new CompositeWhereClause()
                                {
                                    Logic = WhereLogic.Or,
                                    Clauses = new IWhereClause[]{
                                        new WhereClause(){Column = new Field{Name = ""per_nom""}, Operator = WhereOperator.EqualTo, Constraint = ""dupont""},
                                        new WhereClause(){Column = new Field{Name = ""per_nom""}, Operator = WhereOperator.EqualTo, Constraint = ""durant""}
                                    }
                                },
                                new CompositeWhereClause()
                                {
                                    Logic = WhereLogic.Or,
                                    Clauses = new IWhereClause[]{
                                        new WhereClause(){Column = new Field{Name = ""per_age""}, Operator = WhereOperator.GreaterThanOrEqualTo, Constraint = 15},
                                        new WhereClause(){Column = new Field{Name = ""per_age""}, Operator = WhereOperator.LessThan, Constraint = 18}
                                    }
                                } 
                        }
                    })")
                        .SetCategory("Postgresql")
                        .Returns(
                            @"SELECT ""prenom"" FROM ""t_person"" ""p"" WHERE (((""per_nom"" = 'dupont') OR (""per_nom"" = 'durant')) AND ((""per_age"" >= 15) OR (""per_age"" < 18)))")
                        ;


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field() {Name = "prenom"}
                        },

                        From = new ITable[]
                        {
                            new Table() {Name = "t_person", Alias = "p"}
                        },
                        Where = new CompositeWhereClause()
                        {
                            Logic = ClauseLogic.And,
                            Clauses = new IWhereClause[]
                            {
                                new WhereClause()
                                {
                                    Column = new Field {Name = "per_nom"},
                                    Operator = ClauseOperator.EqualTo,
                                    Constraint = "dupont"
                                },
                                new CompositeWhereClause()
                                {
                                    Logic = ClauseLogic.Or,
                                    Clauses = new IWhereClause[]
                                    {
                                        new WhereClause()
                                        {
                                            Column = new Field {Name = "per_age"},
                                            Operator = ClauseOperator.GreaterThanOrEqualTo,
                                            Constraint = 15
                                        },
                                        new WhereClause()
                                        {
                                            Column = new Field {Name = "per_age"},
                                            Operator = ClauseOperator.LessThan,
                                            Constraint = 18
                                        }
                                    }
                                }
                            }
                        }
                    }, false)
                        .SetName("SELECT with WhereClause + Composite clause")
                        .SetCategory("Postgresql")
                        .Returns(
                            @"SELECT ""prenom"" FROM ""t_person"" ""p"" WHERE ((""per_nom"" = 'dupont') AND ((""per_age"" >= 15) OR (""per_age"" < 18)))")
                        ;


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new List<IColumn>()
                        {
                            new Field() {Name = "p.Name", Alias = "Nom complet"},
                            new Field() {Name = "c.Name", Alias = "Civilité"}
                        },
                        From = new ITable[] {new Table() {Name = "person", Alias = "p"}},
                        Joins = new IJoin[]
                        {
                            new InnerJoin(new Table() {Name = "Civility", Alias = "c"},
                                new WhereClause()
                                {
                                    Column = new Field() {Name = "c.Id"},
                                    Operator = ClauseOperator.EqualTo,
                                    Constraint = "p.CivilityId".Field()
                                })
                        }
                    }, false)
                        .SetName("Select with one inner join")
                        .SetCategory("Postgresql")
                        .Returns(
                            @"SELECT ""p"".""Name"" ""Nom complet"", ""c"".""Name"" ""Civilité"" FROM ""person"" ""p"" INNER JOIN ""Civility"" ""c"" ON (""c"".""Id"" = ""p"".""CivilityId"")")
                        ;


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new List<IColumn>()
                        {
                            new Field() {Name = "p.Name", Alias = "Nom complet"},
                            new SelectColumn()
                            {
                                SelectQuery = new SelectQuery()
                                {
                                    Select = new IColumn[] {new Field() {Name = "c.Name", Alias = "Civilité"}},
                                    From = new ITable[] {new Table() {Name = "Civility", Alias = "c"}},
                                    Where =
                                        new WhereClause()
                                        {
                                            Column = "p.CivilityId".Field(),
                                            Operator = ClauseOperator.EqualTo,
                                            Constraint = "c.Id".Field()
                                        }
                                }
                            }
                        },
                        From = new ITable[] {new Table() {Name = "person", Alias = "p"}},

                    }, false)
                        .SetName(
                            @"SELECT <Field {Name = ""col1""}, (SELECT <Field {Name = ""col2"" FROM Table2 WHERE Table1.col = Table2.col) AS alias FROM Table1")
                        .SetCategory("Postgresql")
                        .Returns(
                            @"SELECT ""p"".""Name"" ""Nom complet"", (SELECT ""c"".""Name"" ""Civilité"" FROM ""Civility"" ""c"" WHERE (""p"".""CivilityId"" = ""c"".""Id"")) FROM ""person"" ""p""")
                        ;


                    yield return new TestCaseData(new SelectIntoQuery()
                    {
                        Into = new Table() {Name = "destination"},
                        From = new Table() {Name = "source"}
                    }, false)
                        .SetName("SELECT into from list of table")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT * INTO ""destination"" FROM ""source""");

                    yield return new TestCaseData(new SelectIntoQuery()
                    {
                        Into = new Table() {Name = "destination"},
                        From = new Table() {Name = "source"}
                    }, false)
                        .SetName("SELECT into <Table> from <Table>")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT * INTO ""destination"" FROM ""source""");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new SelectColumn()
                            {
                                SelectQuery = new SelectQuery()
                                {
                                    Select = new IColumn[]
                                    {
                                        new LiteralColumn {Value = 1},
                                    }
                                }
                            }
                        }
                    }, false)
                        .SetCategory("Postgresql")
                        .SetName(@"""SELECT <SelectColumn(
                        {
                            SelectQuery = new SelectQuery(){
                                Select = new IColumn[]
                                {
                                    new LiteralColumn {Value = 1},
                                }
                            }
                        }>""")
                        .SetCategory("Postgresql")
                        .Returns("SELECT (SELECT 1)");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Concat(
                                "firstname".Field(),
                                " ".Literal(),
                                "lastname".Field()
                            ){ Alias = "fullname"}
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "t_person", Alias = "p"}
                        }
                    }, false)
                        .SetName(
                            @"""SELECT <Concat (FieldColumn.From(""firstname""), LiteralColumn.From("" ""),FieldColumn.From(""lastname"")
                            }> 
                        FROM <Table {Name = ""t_person"", Alias = ""p""}> ")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT ""firstname"" || ' ' || ""lastname"" ""fullname"" FROM ""t_person"" ""p""");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        From = new ITable[]
                        {
                            new SelectTable()
                            {
                                Select = new SelectQuery()
                                {
                                    Select = new IColumn[]
                                    {
                                        "firstname".Field(),
                                        "lastname".Field()
                                    },
                                    From = new ITable[]
                                    {
                                        new Table() {Name = "members"}
                                    }
                                }
                            }
                        }
                    }, false)
                        .SetName("SELECT FROM SelectTable")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT * FROM SELECT ""firstname"", ""lastname"" FROM ""members""");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Null(new Field() {Name = "col1"}, "".Literal())
                        },
                        From = new ITable[]
                        {
                            "Table".Table("t")
                        }

                    }, false)
                        .SetName(@"""SELECT <Null(Field{ Name = ""col1"" }, """")> 
                                FROM <""Table"".Table(""t"")>""")
                        .SetCategory("Postgresql")
                        .Returns(@"SELECT COALESCE(""col1"", '') FROM ""Table"" ""t""");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field() {Name = "Employees.Lastname"},
                            new Count(new Field() {Name = "Orders.OrderID"}, "NumberOfOrders"),
                        },
                        From = new ITable[]
                        {
                            "Orders".Table()
                        },
                        Joins = new IJoin[]
                        {
                            new InnerJoin("Employees".Table(),
                                new WhereClause()
                                {
                                    Column = "Employees.EmployeeID".Field(),
                                    Operator = ClauseOperator.EqualTo,
                                    Constraint = "Orders.EmployeeID".Field()
                                })
                        },
                        Where = new CompositeWhereClause()
                        {
                            Logic = ClauseLogic.Or,
                            Clauses = new IWhereClause[]
                            {
                                
                                new WhereClause()
                                {
                                    Column = new Field() {Name = "Lastname"},
                                    Operator = ClauseOperator.EqualTo,
                                    Constraint = "Wayne"
                                },
                                new WhereClause()
                                {
                                    Column = new Field() {Name = "Lastname"},
                                    Operator = ClauseOperator.EqualTo,
                                    Constraint = "Grayson"
                                }
                            }
                        },
                        Having = new HavingClause(){ Column = new Count("Orders.OrderID".Field()), Operator = ClauseOperator.GreaterThan, Constraint = 25}

                    }, false)
                    .SetName(@"new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field() {Name = ""Employees.Lastname""},
                            new Count(new Field() {Name = ""Orders.OrderID""}, ""NumberOfOrders""),
                        },
                        From = new ITable[]
                        {
                            ""Orders"".Table()
                        },
                        Joins = new IJoin[]
                        {
                            new InnerJoin(""Employees"".Table(),
                                new WhereClause()
                                {
                                    Column = ""Employees.EmployeeID"".Field(),
                                    Operator = WhereOperator.EqualTo,
                                    Constraint = ""Orders.EmployeeID"".Field()
                                })
                        },
                        Where = new CompositeWhereClause()
                        {
                            Logic = WhereLogic.Or,
                            Clauses = new IWhereClause[]
                            {
                                
                                new WhereClause()
                                {
                                    Column = new Field() {Name = ""Lastname""},
                                    Operator = WhereOperator.EqualTo,
                                    Constraint = ""Wayne""
                                },
                                new WhereClause()
                                {
                                    Column = new Field() {Name = ""Lastname""},
                                    Operator = WhereOperator.EqualTo,
                                    Constraint = ""Grayson""
                                }
                            }
                        },
                        Having = new HavingClause(){ Column = new Count(""Orders.OrderID"".Field()), Operator = WhereOperator.GreaterThan, ColumnBase = 25}
                    }")
                    .Returns(@"SELECT ""Employees"".""Lastname"", COUNT(""Orders"".""OrderID"") ""NumberOfOrders"" FROM ""Orders"" INNER JOIN ""Employees"" ON (""Employees"".""EmployeeID"" = ""Orders"".""EmployeeID"") WHERE ((""Lastname"" = 'Wayne') OR (""Lastname"" = 'Grayson')) GROUP BY ""Employees"".""Lastname"" HAVING (COUNT(""Orders"".""OrderID"") > 25)");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field() {Name = "Employees.Lastname"},
                            new Count(new Field() {Name = "Orders.OrderID"}, "NumberOfOrders"),
                        },
                        From = new ITable[]
                        {
                            "Orders".Table()
                        },
                        Joins = new IJoin[]
                        {
                            new InnerJoin("Employees".Table(),
                                new WhereClause()
                                {
                                    Column = "Employees.EmployeeID".Field(),
                                    Operator = ClauseOperator.EqualTo,
                                    Constraint = "Orders.EmployeeID".Field()
                                })
                        },
                        Where = new CompositeWhereClause()
                        {
                            Logic = ClauseLogic.Or,
                            Clauses = new IWhereClause[]
                            {
                                
                                new WhereClause()
                                {
                                    Column = new Field() {Name = "Lastname"},
                                    Operator = ClauseOperator.EqualTo,
                                    Constraint = "Wayne"
                                },
                                new WhereClause()
                                {
                                    Column = new Field() {Name = "Lastname"},
                                    Operator = ClauseOperator.EqualTo,
                                    Constraint = "Grayson"
                                }
                            }
                        },
                        Having = new HavingClause() { Column = new Count("Orders.OrderID".Field()), Operator = ClauseOperator.GreaterThan, Constraint = 25 }

                    }, false)
                    .SetName("new SelectQuery()" +
                             "{" +
                                "Select = new IColumn[]" +
                                "{" +
                                    "new Field() {Name = \"Employees.Lastname\"}," +
                                    "new Count(new Field() {Name = \"Orders.OrderID\"}, \"NumberOfOrders\")," +
                                "}," +
                                "From = new ITable[]" +
                                "{" +
                                    "\"Orders\".Table()" +
                                "}," +
                                "Joins = new IJoin[]" +
                             "{" + 
                                "new InnerJoin(\"Employees\".Table(), new WhereClause()\r\n{\r\nColumn = \"Employees.EmployeeID\".Field(),\r\nOperator = WhereOperator.EqualTo,\r\nConstraint = \"Orders.EmployeeID\".Field()\r\n})\r\n},\r\nWhere = new CompositeWhereClause()\r\n{\r\nLogic = WhereLogic.Or,\r\nClauses = new IWhereClause[]\r\n{\r\n\r\nnew WhereClause()\r\n{\r\nColumn = new Field() {Name = \"Lastname\"},\r\nOperator = WhereOperator.EqualTo,\r\nConstraint = \"Wayne\"\r\n},\r\nnew WhereClause()\r\n{\r\nColumn = new Field() {Name = \"Lastname\"},\r\nOperator = WhereOperator.EqualTo,\r\nConstraint = \"Grayson\"\r\n}\r\n}\r\n},\r\nHaving = new HavingClause(){ Column = new Count(\"Orders.OrderID\".Field()), Operator = WhereOperator.GreaterThan, ColumnBase = 25}\r\n}")
                    .Returns(@"SELECT ""Employees"".""Lastname"", COUNT(""Orders"".""OrderID"") ""NumberOfOrders"" FROM ""Orders"" INNER JOIN ""Employees"" ON (""Employees"".""EmployeeID"" = ""Orders"".""EmployeeID"") WHERE ((""Lastname"" = 'Wayne') OR (""Lastname"" = 'Grayson')) GROUP BY ""Employees"".""Lastname"" HAVING (COUNT(""Orders"".""OrderID"") > 25)");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[] { "Col1".Field() },
                        From = new ITable[] { "Table".Table("t") },
                        Limit = 3
                    }, false)
                    .SetName("SelectQuery {Select = new IColumn[] {\"Col1\".Field()}, From = new ITable[] { \"Table\".Table(\"t\") }, Limit = 3}")
                    .SetCategory("SQL Server")
                    .Returns(@"SELECT ""Col1"" FROM ""Table"" ""t"" LIMIT 3");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[] { "Col1".Field() },
                        From = new ITable[] { "Table".Table("t") },
                        Limit = 3
                    }, true)
                    .SetName("Pretty print SelectQuery {Select = new IColumn[] {\"Col1\".Field()}, From = new ITable[] { \"Table\".Table(\"t\") }, Limit = 3}")
                    .SetCategory("SQL Server")
                    .Returns("SELECT \"Col1\" \r\n" +
                             "FROM \"Table\" \"t\" \r\n" +
                             "LIMIT 3");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[] { "Col1".Field() },
                        From = new ITable[] { "Table".Table("t") },
                        Limit = 3,
                        OrderBy = new ISort[] { new SortExpression("Col1") }
                    }, false)
                    .SetName("SelectQuery {Select = new IColumn[] {\"Col1\".Field()}, From = new ITable[] { \"Table\".Table(\"t\") }, Limit = 3, OrderBy = new ISort[]{ new SortExpression(\"Col1\")  }}")
                    .SetCategory("SQL Server")
                    .Returns(@"SELECT ""Col1"" FROM ""Table"" ""t"" ORDER BY ""Col1"" LIMIT 3");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[] { "Col1".Field() },
                        From = new ITable[] { "Table".Table("t") },
                        Limit = 3,
                        OrderBy = new ISort[] { new SortExpression("Col1") }
                    }, true)
                    .SetName("Pretty print SelectQuery {Select = new IColumn[] {\"Col1\".Field()}, From = new ITable[] { \"Table\".Table(\"t\") }, Limit = 3}")
                    .SetCategory("SQL Server")
                    .Returns("SELECT \"Col1\" \r\n" +
                             "FROM \"Table\" \"t\" \r\n" +
                             "ORDER BY \"Col1\" \r\n" +
                             "LIMIT 3");

                }
            }

            public IEnumerable<ITestCaseData> UpdateTestsCases
            {
                get
                {
                    
                    yield return new TestCaseData(new UpdateQuery()
                    {
                        Table = new Table { Name = "Table" },
                        Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = "col2".Field(), Source = "col1".Field()}
                        }
                    }, false)
                    .SetName("\"UPDATE <tablename> SET <destination> = <source>\" where <destination> and <source> are table columns")
                    .SetCategory("Postgresql")
                    .Returns(@"UPDATE ""Table"" SET ""col2"" = ""col1""");

                    yield return new TestCaseData(new UpdateQuery()
                    {
                        Table = new Table { Name = "Table" },
                        Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = "col2".Field(), Source = "col1"}
                        }
                    }, false)
                    .SetName(@"UPDATE <Table {Name = ""Table""}> SET <Set = new[] { new UpdateFieldValue(){ Destination = FieldColumn.From(""col2""), Source = ""col1""}}""")
                    .SetCategory("Postgresql")
                    .Returns(@"UPDATE ""Table"" SET ""col2"" = 'col1'");

                    yield return new TestCaseData(new UpdateQuery()
                    {
                        Table = new Table { Name = "Table" },
                        Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = "col2".Field(), Source = 1}
                        }
                    }, false)
                    .SetName(@"UPDATE <Table {Name = ""Table""}> SET <Set = new[] { new UpdateFieldValue(){ Destination = FieldColumn.From(""col2""), Source = 1""")
                    .SetCategory("Postgresql")
                    .Returns(@"UPDATE ""Table"" SET ""col2"" = 1");


                    yield return new TestCaseData(new UpdateQuery()
                    {
                        Table = new Table { Name = "Table" },
                        Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = "col2".Field(), Source = -1}
                        }
                    }, false)
                    .SetName(@"UPDATE <Table {Name = ""Table""}> SET <Set = new[] { new UpdateFieldValue(){ Destination = FieldColumn.From(""col2""), Source = -1 }}""")
                    .SetCategory("Postgresql")
                    .Returns(@"UPDATE ""Table"" SET ""col2"" = -1");

                    yield return new TestCaseData(new UpdateQuery()
                    {
                        Table = new Table { Name = "Table" },
                        Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = "col2".Field(), Source = "col1".Field()}
                        }
                    }, false)
                                .SetName(@"UPDATE <Table {Name = ""Table""}> SET <Set = new[] { new UpdateFieldValue(){ Destination = FieldColumn.From(""col2""), Source = FieldColumn.From(""col1"")}}""")
                                .SetCategory("Postgresql")
                                .Returns(@"UPDATE ""Table"" SET ""col2"" = ""col1""");


                    yield return new TestCaseData(new UpdateQuery()
                    {
                        Table = "Table".Table() ,
                        Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = "col2".Field(), Source = "col1".Field()}
                        }
                    }, true)
                                .SetName(@"PRETTY PRINT UPDATE <Table {Name = ""Table""}> SET <Set = new[] { new UpdateFieldValue(){ Destination = FieldColumn.From(""col2""), Source = FieldColumn.From(""col1"")}}""")
                                .SetCategory("Postgresql")
                                .Returns("UPDATE \"Table\" \r\n" +
                                         "SET \"col2\" = \"col1\"");
                }
            }

            public IEnumerable<ITestCaseData> DeleteTestsCases
            {
                get
                {
                    yield return new TestCaseData(new DeleteQuery(){Table = "table".Table()}, false)
                        .SetName(@"new DeleteQuery(){Table = ""table"".Table()}")
                        .Returns(@"DELETE FROM ""table""");


                    yield return new TestCaseData(new DeleteQuery() { Table = "table".Table() }, true)
                        .SetName(@"PRETTY PRINT : new DeleteQuery(){Table = ""table"".Table()}")
                        .Returns(@"DELETE FROM ""table""");

                    yield return new TestCaseData(new DeleteQuery()
                    {
                        Table = "city".Table(),
                        Where = new WhereClause()
                        {
                            Column = "name".Field(),
                            Operator = ClauseOperator.NotEqualTo,
                            Constraint = "Gotham"
                        }
                    }, true)
                        .SetName(
                            @"PRETTY PRINT : new DeleteQuery(){Table = ""table"".Table(), Where = new WhereClause() {Column = ""name"".Field(), Operator = ClauseOperator.NotEqualTo, Constraint = ""Gotham""}")
                        .Returns(
                            "DELETE FROM \"city\" \r\n" +
                            "WHERE (\"name\" <> \'Gotham\')");



                    yield return new TestCaseData(new DeleteQuery(), false)
                        .SetName(@"new DeleteQuery()")
                        .Throws(typeof(InvalidQueryException));
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
        public string Select(SelectQueryBase selectQuery, bool prettyPrint)
        {
            _renderer.PrettyPrint = prettyPrint;

            return _renderer.Render(selectQuery);
        }

        [TestCaseSource(typeof(Cases), "UpdateTestsCases")]
        public string Update(UpdateQuery updateQuery, bool prettyPrint)

        {
            _renderer.PrettyPrint = prettyPrint;
            return _renderer.Render(updateQuery);
        }

        [TestCaseSource(typeof(Cases), "DeleteTestsCases")]
        public string Delete(DeleteQuery deleteQuery, bool prettyPrint)
        {
            _renderer.PrettyPrint = prettyPrint;
            return _renderer.Render(deleteQuery);
        }
    }
}