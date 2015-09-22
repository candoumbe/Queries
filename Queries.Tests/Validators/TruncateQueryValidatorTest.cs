using System.Collections.Generic;
using NUnit.Framework;
using Queries.Builders;
using Queries.Validators;

namespace Queries.Tests.Validators
{
    public class TruncateQueryValidatorTest
    {

        private class Cases
        {
            public IEnumerable<ITestCaseData> IsValidTestCases
            {
                get
                {
                    yield return new TestCaseData(null)
                        .SetName("null")
                        .Returns(false);

                    yield return new TestCaseData(new TruncateQuery())
                        .SetName("new TrunctateQuery()")
                        .Returns(false);

                    yield return new TestCaseData(new TruncateQuery(){Name = "people"})
                        .SetName("TRUNCATE TABLE [people]")
                        .Returns(true);

                }
            }
        }

        [TestCaseSource(typeof(Cases), nameof(Cases.IsValidTestCases))]
        public bool IsValid(TruncateQuery query)
        {
            return new TruncateQueryValidator().IsValid(query);
        }
    }
}