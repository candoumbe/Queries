using System.Collections.Generic;
using NUnit.Framework;
using Queries.Builders;
using Queries.Extensions;
using Queries.Parts;
using Queries.Parts.Clauses;
using Queries.Parts.Columns;
using Queries.Parts.Joins;
using Queries.Validators;

namespace Queries.Tests.Validators
{
    public class SelectQueryValidatorTest
    {
        private IValidate<SelectQuery> _validator;

        private class Cases
        {
            public IEnumerable<ITestCaseData> ValidateSelectQueryTestCases
            {
                get
                {
                    yield return new TestCaseData(new SelectQuery())
                        .SetName("An empty SelectQuery is not valid")
                        .Returns(false);

                    yield return new TestCaseData(null)
                        .SetName("A null SelectQuery is not valid")
                        .Returns(false);


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Field(){Name = "col 1"}
                        }
                    })
                    .SetName("\"SELECT [col 1]\" is a valid SelectQuery")
                    .Returns(true);

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new LiteralColumn(){Value = "col 1"}
                        }
                    })
                    .SetName("\"SELECT 'col 1' is a valid SelectQuery")
                    .Returns(true);


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
                                        new LiteralColumn{ Value = 1}, 
                                    }
                                }
                            }
                        }
                    })
                    .SetName("\"SELECT (SELECT 1)\" is a valid SelectQuery")
                    .Returns(true);


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
                        Having =
                            new HavingClause()
                            {
                                Column = new Count("Orders.OrderID".Field()),
                                Operator = ClauseOperator.GreaterThan,
                                Constraint = 25
                            }

                    })
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
                        .Returns(true);


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
                        }
                    })
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
                    }")
                    .Returns(true);


                }
            }

        }

        [SetUp]
        public void SetUp()
        {
            _validator = new SelectQueryValidator();
        }

        [TearDown]
        public void TearDown()
        {
            _validator = null;
        }


        [TestCaseSource(typeof(Cases), "ValidateSelectQueryTestCases")]
        public bool ValidateSelectQuery(SelectQuery selectQuery)
        {
            return _validator.IsValid(selectQuery);
        }
    }
}
