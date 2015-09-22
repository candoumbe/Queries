using System.Collections.Generic;
using NUnit.Framework;
using Queries.Builders;
using static Queries.Builders.Fluent.QueryBuilder;
using Queries.Extensions;
using Queries.Parts.Columns;
using Queries.Validators;

namespace Queries.Tests.Validators
{
    public class CreateViewQueryValidatorTest
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

                    yield return new TestCaseData(CreateView(null).Build())
                        .SetName($"{nameof(CreateView)}(null)")
                        .Returns(false);

                    yield return new TestCaseData(
                        CreateView("people")
                            .As(
                                Select(Concat("Firstname".Field(), " ".Literal(), "Lastname".Field()))
                                    .From("user".Table())
                                    .Build())
                            .Build())
                        .SetName("CREATE VIEW [people] \r\n" +
                                 "AS \r\n" +
                                 "SELECT [Firstname] + ' ' + [Lastname] \r\n" +
                                 "FROM [User]")
                        .Returns(true);

                    yield return new TestCaseData(
                        CreateView("people")
                            .As(
                                Select(Concat("fullname", "Firstname".Field(), " ".Literal(), "Lastname".Field()))
                                    .From("user".Table())
                                    .Build())
                            .Build())
                        .SetName("CREATE VIEW [people] \r\n" +
                                 "AS \r\n" +
                                 "SELECT [Firstname] + ' ' + [Lastname] \r\n" +
                                 "FROM [User]")
                        .Returns(true);
                }
            }
        }

        [TestCaseSource(typeof(Cases), nameof(Cases.IsValidTestCases))]
        public bool IsValid(CreateViewQuery query)
        {
            return new CreateViewQueryValidator().IsValid(query);
        }
    }
}
