﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Queries.Builders;
using Queries.Builders.Fluent;
using Queries.Parts;
using Queries.Parts.Columns;
using Queries.Validators;

namespace Queries.Tests.Validators
{
    public class ValidatorsTest
    {
        private class Cases
        {
            public IEnumerable<ITestCaseData> TableValidatorTestCases
            {
                get
                {
                    #region Table
                    yield return new TestCaseData(new Table())
                                    .SetName(@"""new Table()"" is not valid ")
                                    .SetCategory("Table")
                                    .Returns(false);


                    yield return new TestCaseData(null)
                        .SetName("null is not a valid Table")
                        .SetCategory("Table")
                        .Returns(false);

                    yield return new TestCaseData(new Table { Alias = "Alias" })
                        .SetName(@"""new Table{Alias = ""Alias"" is not valid")
                        .SetCategory("Table")
                        .Returns(false);

                    yield return new TestCaseData(new Table { Name = "Table" })
                        .SetName(@"A table term where only the name is specified is valid")
                        .SetCategory("Table")
                        .Returns(true);

                    yield return new TestCaseData(new Table { Name = null })
                        .SetName(@" table term where only the name is explicitely set to NULL is not valid")
                        .SetCategory("Table")
                        .Returns(false);

                    yield return new TestCaseData(new Table { Name = "" })
                        .SetName(@"""new Table { Name = """" } is not valid")
                        .SetCategory("Table")
                        .Returns(false);

                    yield return new TestCaseData(new Table { Name = String.Empty })
                        .SetName(@"""new Table { Name = String.Empty }"" is not valid")
                        .SetCategory("Table")
                        .Returns(false);

                    yield return new TestCaseData(new Table { Name = null, Alias = null })
                        .SetName(@"""new Table { Name = null, Alias = null}"" is not valid")
                        .SetCategory("Table")
                        .Returns(false);

                    yield return new TestCaseData(new Table { Name = null, Alias = "" })
                        .SetName(@"""new Table { Name = null, Alias = """" }"" is not valid")
                        .SetCategory("Table")
                        .Returns(false);

                    yield return new TestCaseData(new Table { Name = null, Alias = String.Empty })
                        .SetName(@"""new Table { Name = null, Alias = String.Empty }"" is not valid")
                        .SetCategory("Table")
                        .Returns(false); 
                    #endregion

                    #region SelectTable
                    yield return new TestCaseData(new SelectTable())
                                   .SetName(@"""new SelectTable()"" is not valid")
                                   .SetCategory("SelectTable")
                                   .Returns(false);

                    yield return new TestCaseData(new SelectTable() { Select = null })
                        .SetName(@"""new SelectTable(){ Select = null }"" is not valid")
                        .SetCategory("SelectTable")
                        .Returns(false);

                    yield return new TestCaseData(new SelectTable() { Select = new Select(new NumericColumn(1)).Build() })
                        .SetName(@"""new SelectTable(){ Select = new Select(new NumericColumn(1)).Build() }"" is valid")
                        .SetCategory("SelectTable")
                        .Returns(true);

                    yield return new TestCaseData(new SelectTable() { Alias = String.Empty })
                        .SetName(@"""new SelectTable(){ Alias = String.Empty }"" is not valid")
                        .SetCategory("SelectTable")
                        .Returns(false);

                    yield return new TestCaseData(new SelectTable() { Select = new Select("col1", "col2").Build()})
                        .SetName(@"""new SelectTable(){Select = new Select(""col1"", ""col2"").Build() }"" is valid")
                        .SetCategory("SelectTable")
                        .Returns(true);

                    #endregion
                }
            }

            public IEnumerable<ITestCaseData> ColumnValidatorTestCases
            {
                get
                {
                    yield return new TestCaseData(new LiteralColumn())
                        .SetName(@"""new LiteralColumn()"" is valid ")
                        .SetCategory("LiteralColumn")
                        .Returns(true);

                    yield return new TestCaseData(null)
                        .SetName("null is not valid")
                        .SetCategory("LiteralColumn")
                        .Returns(false);

                    yield return new TestCaseData(new LiteralColumn() {Alias = "Alias"})
                        .SetName(@"""new LiteralColumn{Alias = ""Alias""}"" is valid")
                        .SetCategory("LiteralColumn")
                        .Returns(true);

                    yield return new TestCaseData(new LiteralColumn() {Value = null})
                        .SetName(@"""new LiteralColumn() { Value = null }"" is valid")
                        .SetCategory("LiteralColumn")
                        .Returns(true);

                    yield return new TestCaseData(new Min("columnName"))
                        .SetName(@"""new Min(""columnName"")"" is valid")
                        .SetCategory("AggregateColumn")
                        .Returns(true);

                    yield return new TestCaseData(new Min(""))
                        .SetName(@"""new Min("""")"" is not valid")
                        .SetCategory("AggregateColumn")
                        .Returns(false);

                    yield return new TestCaseData(new Min(" "))
                        .SetName(@"""new Min("" "")"" is not valid")
                        .SetCategory("AggregateColumn")
                        .Returns(false);

                    yield return new TestCaseData(new Min(String.Empty))
                        .SetName(@"""new Min(String.Empty)"" is not valid")
                        .SetCategory("AggregateColumn")
                        .Returns(false);
                }
            }

        }

        [TestCaseSource(typeof(Cases), "TableValidatorTestCases")]
        public bool ValidateTable(ITable selectQuery)
        {
            return new TableValidator().Validate(selectQuery);
        }

        [TestCaseSource(typeof(Cases), "ColumnValidatorTestCases")]
        public bool ValidateColumn(IColumn column)
        {
            return new ColumnValidator().Validate(column);
        }
    }
}