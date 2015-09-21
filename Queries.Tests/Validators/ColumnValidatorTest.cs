using System;
using System.Collections.Generic;
using NUnit.Framework;
using Queries.Extensions;
using Queries.Parts.Columns;
using Queries.Validators;

namespace Queries.Tests.Validators
{
    public class ColumnValidatorTest
    {
        private IValidate<IColumn> _columnValidator;


        private class Cases 
        {
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


                    
                    yield return new TestCaseData(new Null(1.Literal(), 1.Literal()))
                        .SetName(@"""new Null(1.Literal(), 1.Literal())"" is valid")
                        .SetCategory("NullColumn")
                        .Returns(true);

                    yield return new TestCaseData(new Length("".Field()))
                        .SetName(@"""new Length("""".Field()) not is valid")
                        .SetCategory("LengthColumn")
                        .Returns(false);

                    

                    yield return new TestCaseData(new Length("firstname".Field()))
                        .SetName(@"""new Length(""firstname"".Field()) not is valid")
                        .SetCategory("LengthColumn")
                        .Returns(true);
                }
            }
        }


        [SetUp]
        public void SetUp()
        {
            _columnValidator = new ColumnValidator();
        }

        [TearDown]
        public void TearDown()
        {
            _columnValidator = null;
        }

        [TestCaseSource(typeof(Cases), "ColumnValidatorTestCases")]
        public bool Validate(IColumn column)
        {
            return _columnValidator.IsValid(column);
        }
    }
}