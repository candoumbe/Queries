using System;
using System.Collections.Generic;
using NUnit.Framework;
using Queries.Builders;
using Queries.Parts;
using Queries.Parts.Clauses;
using Queries.Parts.Columns;
using Queries.Parts.Joins;
using Queries.Renderers;

namespace Queries.Tests.Renderers
{
    public class SqlServerRendererTest
    {
        private SqlServerRenderer _renderer;


        private class Cases
        {
            public IEnumerable<ITestCaseData> SelectTestsCases
            {
                get
                {
                    yield return new TestCaseData(null)
                                .SetName("Select null query")
                                .SetCategory("SQL Server")
                                .Returns(String.Empty);


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field(){Name = "Col1"}
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    })
                    .SetName(@"""SELECT <Field{ Name = ""Col1"" }> 
                                FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL Server")
                    .Returns("SELECT [Col1] FROM [Table] [t]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new NullColumn(new Field(){Name = "col1"}, LiteralColumn.From(String.Empty))
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    })
                    .SetName(@"""SELECT <Null(Field{ Name = ""col1"" }, """")> 
                                FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL Server")
                    .Returns("SELECT ISNULL([col1], '') FROM [Table] [t]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Concat(
                                new NullColumn(new Field(){Name = "firstname"}, LiteralColumn.From(String.Empty)),
                                LiteralColumn.From(" "),
                                new NullColumn(new Field(){Name = "lastname"}, LiteralColumn.From(String.Empty))
                            ) {Alias = "fullname"}
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    })
                    .SetName(@"""SELECT <Concat(Null(Field{ Name = ""firstname"" }, """"), LiteralColumn.From("" ""), Null(Field{ Name = ""lastname"" }, """")){ Alias = ""fullname"" }> 
                                FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL Server")
                    .Returns("SELECT ISNULL([firstname], '') + ' ' + ISNULL([lastname], '') AS [fullname] FROM [Table] [t]");



                    yield return new TestCaseData(new SelectQuery()
                    {
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    })
                    .SetName(@"""SELECT FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL Server")
                    .Returns("SELECT * FROM [Table] [t]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = null,
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    })
                    .SetName("Select with no column specified and one table in the FROM part")
                    .SetCategory("SQL Server")
                    .Returns("SELECT * FROM [Table] [t]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field(){Name = "Col1"}
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "dbo.Table", Alias = "t"}
                        },

                    })
                    .SetName(@"""SELECT <Field{ Name = ""Col1"" }> FROM <Table { Name = ""dbo.Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL Server")
                    .Returns("SELECT [Col1] FROM [dbo].[Table] [t]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field(){Name = "Col1"},
                            new Field(){Name = "Col2"}
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table1", Alias = "t1"},
                            new Table(){Name = "Table2", Alias = "t2"}
                        },

                    })
                    .SetName(@"""SELECT <Field{ Name = ""Col1"" }>, <Field{ Name = ""Col2"" }> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL Server")
                    .Returns("SELECT [Col1], [Col2] FROM [Table1] [t1], [Table2] [t2]");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field(){Name = "Col1", Alias = "Alias"}
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    })
                    .SetName(@"""SELECT <Field{ Name = ""Col1"", Alias = ""Alias"" }> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL Server")
                    .Returns("SELECT [Col1] AS [Alias] FROM [Table] [t]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field(){Name = "Col1"},
                            new Field(){Name = "Col2"},
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    })
                    .SetName(@"""SELECT <Field{ Name = ""Col1"" }>, <Field{ Name = ""Col2"" }> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL Server")
                    .Returns("SELECT [Col1], [Col2] FROM [Table] [t]");




                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min (new Field() {Name = "Col1"})
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    })
                    .SetName(@"""SELECT <Min(new Field() {Name = ""Col1""})> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL Server")
                    .Returns("SELECT MIN([Col1]) FROM [Table] [t]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min ("Col1")
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    })
                    .SetName(@"""SELECT <new Min (""Col1"")> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL Server")
                    .Returns("SELECT MIN([Col1]) FROM [Table] [t]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new Field(){Name = "Col1"})
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    })
                        .SetName(@"""SELECT <Min(Field {Name = ""Col1""})> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("SQL Server")
                    .Returns("SELECT MIN([Col1]) FROM [Table] [t]");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new Field(){Name = "Col1", Alias = "Alias"})
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    })
                    .SetName(@"""SELECT <Min(Field {Name = ""Col1"", Alias = ""Alias""})> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL Server")
                    .Returns("SELECT MIN([Col1]) FROM [Table] [t]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new Field(){Name = "Col1"}, "Alias")
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    })
                    .SetName(@"""SELECT <Min(Field {Name = ""Col1""}, ""Alias"")> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL Server")
                    .Returns("SELECT MIN([Col1]) AS [Alias] FROM [Table] [t]");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new Field(){Name = "Col1"}, "Minimum"),
                            new Max(new Field(){Name = "Col2", Alias = "Maximum"})
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    })
                    .SetName(@"""SELECT <Min(Field {Name = ""Col1""}, ""Minimum""})>, <Max(Field {Name = ""Col2"", Alias = ""Maximum""})> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL Server")
                    .Returns("SELECT MIN([Col1]) AS [Minimum], MAX([Col2]) FROM [Table] [t]");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new Field(){Name = "Col1", Alias = "Minimum"}),
                            new Field(){Name = "Col2", Alias = "Maximum"}
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    })
                    .SetName(@"""SELECT <Min(Field {Name = ""Col1"", Alias = ""Minimum""})>, <Field {Name = ""Col2"", Alias = ""Maximum"")> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL Server")
                    .SetDescription("This should generate a GROUP BY clause")
                    .Returns("SELECT MIN([Col1]), [Col2] AS [Maximum] FROM [Table] [t] GROUP BY [Col2]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new Field(){Name = "Col1", Alias = "Minimum"}),
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },
                        Union = new[]{
                           new SelectQuery()
                           { 
                                Select = new IColumn[]
                                {
                                    new Min(new Field(){Name = "Col2", Alias = "Minimum"}),
                                },
                                From = new ITable[]
                                {
                                    new Table(){Name = "Table2", Alias = "t2"}
                                }
                           }
                        }
                    })
                   .SetName(@"""SELECT <Min(Field {Name = ""Col1"", Alias = ""Minimum""})> FROM <Table { Name = ""Table"", Alias = ""t"" }> UNION SELECT <Min(Field {Name = ""Col2"", Alias = ""Minimum""})> FROM <Table { Name = ""Table2"", Alias = ""t2"" }>""")
                   .SetCategory("SQL Server")
                    .Returns("SELECT MIN([Col1]) FROM [Table] [t] UNION SELECT MIN([Col2]) FROM [Table2] [t2]");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field(){ Name = "civ_prenom"}
                        },

                        From = new ITable[]
                        {
                            new Table(){ Name =  "t_civilite", Alias = "civ"}
                        },
                        Where = new WhereClause()
                        {
                            Column = FieldColumn.From("civ_nom"),
                            Operator = ClauseOperator.EqualTo,
                            Constraint = "dupont"
                        }
                    })
                    .SetName(@"""SELECT <Field {Name = ""civ_prenom""}> FROM <Table { Name = ""t_civilite"", Alias = ""civ"" }> WHERE <WhereClause { Column = FieldColumn.From(""civ_nom""),Operator = WhereOperator.EqualTo, Constraint = ""dupont""})>")
                    .SetCategory("SQL Server")
                    .Returns("SELECT [civ_prenom] FROM [t_civilite] [civ] WHERE ([civ_nom] = 'dupont')");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            FieldColumn.From("prenom")
                        },
                        From = new ITable[]
                        {
                            new Table(){ Name =  "t_person", Alias = "p"}
                        },
                        Where = new CompositeWhereClause()
                        {
                            Logic = ClauseLogic.Or,
                            Clauses = new List<IWhereClause>()
                            {
                                new WhereClause() {Column = new Field(){ Name = "per_age"}, Operator = ClauseOperator.GreaterThanOrEqualTo, Constraint = 15},
                                new WhereClause() {Column = new Field(){ Name = "per_age"}, Operator = ClauseOperator.LessThan, Constraint = 18}
                            }
                        }
                    })
                    .SetName(@"""SELECT <FieldColumn.From(""prenom"")> FROM <Table { Name = ""t_person"", Alias = ""p"" }> WHERE <CompositeWhereClause {  Logic = WhereLogic.Or, Clauses = new IWhereClause[]{new WhereClause() {Column = new Field(){ Name = ""per_age""}, Operator = WhereOperator.GreaterThanOrEqualTo, Constraint = 15}, new WhereClause(){Column = new Field{Name = ""per_age""}, Operator = WhereOperator.LessThan, Constraint = 18}})")
                    .SetCategory("SQL Server")
                    .Returns("SELECT [prenom] FROM [t_person] [p] WHERE (([per_age] >= 15) OR ([per_age] < 18))");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field(){ Name = "prenom"}
                        },

                        From = new ITable[]
                        {
                            new Table(){ Name =  "t_person", Alias = "p"}
                        },
                        Where = new CompositeWhereClause()
                        {
                            Logic = ClauseLogic.And,
                            Clauses = new IWhereClause[]
                            {
                                new CompositeWhereClause()
                                {
                                    Logic = ClauseLogic.Or,
                                    Clauses = new IWhereClause[]{
                                        new WhereClause(){Column = new Field{Name = "per_nom"}, Operator = ClauseOperator.EqualTo, Constraint = "dupont"},
                                        new WhereClause(){Column = new Field{Name = "per_nom"}, Operator = ClauseOperator.EqualTo, Constraint = "durant"}
                                    }
                                },
                                new CompositeWhereClause()
                                {
                                    Logic = ClauseLogic.Or,
                                    Clauses = new IWhereClause[]{
                                        new WhereClause(){Column = new Field{Name = "per_age"}, Operator = ClauseOperator.GreaterThanOrEqualTo, Constraint = 15},
                                        new WhereClause(){Column = new Field{Name = "per_age"}, Operator = ClauseOperator.LessThan, Constraint = 18}
                                    }
                                }
                            }
                        }
                    })
                    .SetName(@"""SELECT <FieldColumn.From(""prenom"")> FROM <Table { Name = ""t_person"", Alias = ""p"" }> WHERE <CompositeWhereClause {  
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
                    .SetCategory("SQL Server")
                    .Returns("SELECT [prenom] FROM [t_person] [p] WHERE ((([per_nom] = 'dupont') OR ([per_nom] = 'durant')) AND (([per_age] >= 15) OR ([per_age] < 18)))");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field(){ Name = "prenom"}
                        },

                        From = new ITable[]
                        {
                            new Table(){ Name =  "t_person", Alias = "p"}
                        },
                        Where = new CompositeWhereClause()
                        {
                            Logic = ClauseLogic.And,
                            Clauses = new IWhereClause[]
                            {
                                new WhereClause(){Column = new Field{Name = "per_nom"}, Operator = ClauseOperator.EqualTo, Constraint = "dupont"},
                                new CompositeWhereClause()
                                {
                                    Logic = ClauseLogic.Or,
                                    Clauses = new IWhereClause[]{
                                        new WhereClause(){Column = new Field{Name = "per_age"}, Operator = ClauseOperator.GreaterThanOrEqualTo, Constraint = 15},
                                        new WhereClause(){Column = new Field{Name = "per_age"}, Operator = ClauseOperator.LessThan, Constraint = 18}
                                    }
                                }
                            }
                        }
                    })
                    .SetName("SELECT with WhereClause + Composite clause")
                    .SetCategory("SQL Server")
                    .Returns("SELECT [prenom] FROM [t_person] [p] WHERE (([per_nom] = 'dupont') AND (([per_age] >= 15) OR ([per_age] < 18)))");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new List<IColumn>()
                        {
                            new Field(){Name = "p.Name", Alias = "Nom complet"},
                            new Field() {Name = "c.Name", Alias = "Civilité"}
                        },
                        From = new ITable[] { new Table() { Name = "person", Alias = "p" } },
                        Joins = new IJoin[]
                        {
                            new InnerJoin(new Table(){Name = "Civility", Alias = "c"}, 
                                new WhereClause(){ Column = new Field(){Name = "c.Id"}, Operator = ClauseOperator.EqualTo, Constraint = FieldColumn.From("p.CivilityId")})
                        }
                    })
                    .SetName("Select with one inner join")
                    .SetCategory("SQL Server")
                    .Returns("SELECT [p].[Name] AS [Nom complet], [c].[Name] AS [Civilité] FROM [person] [p] INNER JOIN [Civility] [c] ON ([c].[Id] = [p].[CivilityId])");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new List<IColumn>()
                        {
                            new Field(){Name = "p.Name", Alias = "Nom complet"},
                            new SelectColumn()
                            {
                                SelectQuery = new SelectQuery()
                                {
                                    Select = new IColumn[]{new Field() {Name = "c.Name", Alias = "Civilité"}},
                                    From = new ITable[] {new Table(){Name = "Civility", Alias = "c"}},
                                    Where = new WhereClause(){Column = FieldColumn.From("p.CivilityId"), Operator = ClauseOperator.EqualTo, Constraint = FieldColumn.From("c.Id")}
                                }
                            }
                        },
                        From = new ITable[] { new Table() { Name = "person", Alias = "p" } },

                    })
                    .SetName(@"SELECT <Field {Name = ""p.Name"", Alias = ""Nom complet""}>, (SELECT col2 FROM Table2 WHERE Table1.col = Table2.col) AS alias FROM Table1")
                    .SetCategory("SQL Server")
                    .Returns("SELECT [p].[Name] AS [Nom complet], (SELECT [c].[Name] AS [Civilité] FROM [Civility] [c] WHERE ([p].[CivilityId] = [c].[Id])) FROM [person] [p]");


                    yield return new TestCaseData(new SelectIntoQuery()
                    {
                        Into = new Table() { Name = "destination" },
                        From = new Table() { Name = "source" }
                    })
                    .SetName("SELECT into from list of table")
                    .SetCategory("SQL Server")
                    .Returns("SELECT * INTO [destination] FROM [source]");

                    yield return new TestCaseData(new SelectIntoQuery()
                    {
                        Into = new Table() { Name = "destination" },
                        From = new Table() { Name = "source" }
                    })
                    .SetName("SELECT into from list of table")
                    .SetCategory("SQL Server")
                    .Returns("SELECT * INTO [destination] FROM [source]");


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
                    })
                    .SetCategory("SQL Server")
                    .SetName(@"""SELECT <SelectColumn(
                        {
                            SelectQuery = new SelectQuery(){
                                Select = new IColumn[]
                                {
                                    new LiteralColumn {Value = 1},
                                }
                            }
                        }>""")
                    .SetCategory("SQL Server")
                    .Returns("SELECT (SELECT 1)");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Concat(FieldColumn.From("firstname"), LiteralColumn.From(" "), FieldColumn.From("lastname")){Alias = "fullname" }, 
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "t_person", Alias = "p"} 
                        }
                    })
                    .SetName(@"""SELECT <Concat {alias: ""fullname"", columns: new IColumn[]
                            {
                                FieldColumn.From(""firstname""),
                                LiteralColumn.From("" ""),
                                FieldColumn.From(""lastname"")
                            }> 
                        FROM <Table {Name = ""t_person"", Alias = ""p""}> ")
                    .SetCategory("SQL Server")
                    .Returns("SELECT [firstname] + ' ' + [lastname] AS [fullname] FROM [t_person] [p]");
                    


                }
            }
            
            public IEnumerable<ITestCaseData> RenderUpdateTestCases
            {
                get
                {
                    yield return new TestCaseData(new UpdateQuery()
            {
                Table = new Table { Name = "Table" },
                Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = FieldColumn.From("col2"), Source = FieldColumn.From("col1")}
                        }
            })
            .SetName("\"UPDATE <tablename> SET <destination> = <source>\" where <destination> and <source> are table columns")
            .SetCategory("SQL Server")
            .Returns("UPDATE [Table] SET [col2] = [col1]");

                    yield return new TestCaseData(new UpdateQuery()
                    {
                        Table = new Table { Name = "Table" },
                        Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = FieldColumn.From("col2"), Source = "col1"}
                        }
                    })
                    .SetName(@"""UPDATE <tablename> SET <destination> = <source>"" where <destination> is a table column and <source> is a c# string")
                    .SetCategory("SQL Server")
                    .Returns("UPDATE [Table] SET [col2] = 'col1'");

                    yield return new TestCaseData(new UpdateQuery()
                    {
                        Table = new Table { Name = "Table" },
                        Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = FieldColumn.From("col2"), Source = 1}
                        }
                    })
                    .SetName(@"""UPDATE <tablename> SET <destination> = <source>"" where <destination> is a table column and <source> is a c# positive integer")
                    .SetCategory("SQL Server")
                    .Returns("UPDATE [Table] SET [col2] = 1");


                    yield return new TestCaseData(new UpdateQuery()
                    {
                        Table = new Table { Name = "Table" },
                        Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = FieldColumn.From("col2"), Source = -1}
                        }
                    })
                    .SetName(@"""UPDATE <tablename> SET <destination> = <source>"" where <destination> is a table column and <source> is a c# negative integer")
                    .SetCategory("SQL Server")
                    .Returns("UPDATE [Table] SET [col2] = -1");
                    

                }
            }
        }

        [SetUp]
        public void SetUp()
        {
            _renderer = new SqlServerRenderer();
        }

        [TearDown]
        public void TearDown()
        {
            _renderer = null;
        }


        [TestCaseSource(typeof(Cases), "SelectTestsCases")]
        public string Select(SelectQueryBase selectQuery)
        {
            return _renderer.Render(selectQuery);
        }

        [TestCaseSource(typeof(Cases), "RenderUpdateTestCases")]
        public string Update(UpdateQuery updateQuery)
        {
            return _renderer.Render(updateQuery);
        }
    }
}
