using System.Collections.Generic;
using NUnit.Framework;
using Queries.Builders;
using Queries.Parts;
using Queries.Validators;

namespace Queries.Tests.Validators
{
    public class DeleteQueryValidatorTest
    {
        private IValidate<DeleteQuery> _validator;

        private class Cases
        {
            public IEnumerable<ITestCaseData> ValidateTestCases
            {
                get
                {
                    yield return new TestCaseData(null)
                        .SetName(@"""null"" is not valid")
                        .Returns(false);

                    yield return new TestCaseData(new DeleteQuery())
                        .SetName(@"""new DeleteQuery()"" is not valid")
                        .Returns(false);


                    yield return new TestCaseData(new DeleteQuery(){Table = new Table(){Name = "member"}})
                        .SetName(@"""new DeleteQuery(){Table = new Table(){Name = ""member""}}"" is valid")
                        .Returns(true);
                }
            }

        }

        [SetUp]
        public void SetUp()
        {
            _validator = new DeleteQueryValidator();
        }

        [TearDown]
        public void TearDown()
        {
            _validator = null;
        }


        [TestCaseSource(typeof(Cases), "ValidateTestCases")]
        public bool Validate(DeleteQuery deleteQuery)
        {
            return _validator.IsValid(deleteQuery);
        }

    }
}