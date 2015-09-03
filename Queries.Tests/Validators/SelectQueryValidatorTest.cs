using System.Collections.Generic;
using NUnit.Framework;
using Queries.Builders;
using Queries.Parts.Columns;
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
                            new TableColumn(){Name = "col 1"}
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
            return _validator.Validate(selectQuery);
        }
    }
}
