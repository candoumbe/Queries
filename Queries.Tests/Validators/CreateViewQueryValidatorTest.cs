using System.Collections.Generic;
using NUnit.Framework;
using Queries.Builders;
using Queries.Builders.Fluent;
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

                    yield return new TestCaseData(new CreateViewQuery())
                        .SetName("new CreateViewQuery()")
                        .Returns(false);

                    yield return new TestCaseData(new CreateViewQuery()
                    {
                        Name = "people",
                        Select = new Select(new Concat(
                                "Firstname".Field(),
                                " ".Literal(),
                                "Lastname".Field()
                            ))
                            .From("user".Table())
                            .Build()
                    })
                    .SetName("CREATE VIEW [people] \r\n" +
                             "AS \r\n" +
                             "SELECT [Firstname] + ' ' + [Lastname] \r\n" +
                             "FROM [User]")
                    .Returns(true);

                    yield return new TestCaseData(new CreateViewQuery()
                    {
                        Name = "people",
                        Select = new Select(new Concat(
                                "Firstname".Field(),
                                " ".Literal(),
                                "Lastname".Field()) {Alias = "Fullname"})
                            .From("user".Table())
                            .Build()
                    })
                    .SetName("CREATE VIEW [people] \r\n" +
                             "AS \r\n" +
                             "SELECT [Firstname] + ' ' + [Lastname] AS [Fullname] \r\n" +
                             "FROM [User]")
                    .Returns(true);
                }
            }
        }

        [TestCaseSource(typeof(Cases), "IsValidTestCases")]
        public bool IsValid(CreateViewQuery query)
        {
            return new CreateViewQueryValidator().IsValid(query);
        }
    }
}
