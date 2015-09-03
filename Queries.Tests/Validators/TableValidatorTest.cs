using System;
using System.Collections.Generic;
using NUnit.Framework;
using Queries.Builders;
using Queries.Parts;
using Queries.Parts.Columns;
using Queries.Validators;

namespace Queries.Tests.Validators
{
    public class TableValidatorTest
    {
        private IValidate<TableTerm> _validator;

        private class Cases
        {
            public IEnumerable<ITestCaseData> ValidateTableTermTestCases
            {
                get
                {
                    yield return new TestCaseData(new TableTerm())
                        .SetName("A table term where no name nor alias explicitely set is not valid")
                        .Returns(false);


                    yield return new TestCaseData(null)
                        .SetName("a null table term is not valid")
                        .Returns(false);

                    yield return new TestCaseData(new TableTerm{Alias = "Alias"})
                        .SetName("A table term where only the alias is specified is not valid")
                        .Returns(false);

                    yield return new TestCaseData(new TableTerm { Name = "Table" })
                        .SetName("A table term where only the name is specified is valid")
                        .Returns(true);

                    yield return new TestCaseData(new TableTerm { Name = null })
                       .SetName("A table term where only the name is explicitely set to NULL is not valid")
                       .Returns(false);

                    yield return new TestCaseData(new TableTerm { Name = "" })
                       .SetName("A table term where only the name is explicitely set to an \"\" is not valid")
                       .Returns(false);

                    yield return new TestCaseData(new TableTerm { Name = String.Empty })
                       .SetName("A table term where only the name is explicitely set to String.Empty is not valid")
                       .Returns(false);

                    yield return new TestCaseData(new TableTerm { Name = null, Alias = null})
                       .SetName("A table term where name and alias are explicitely set to null is not valid")
                       .Returns(false);

                    yield return new TestCaseData(new TableTerm { Name = null, Alias = "" })
                       .SetName("A table term where name is set to null and alias is '' is not valid")
                       .Returns(false);

                    yield return new TestCaseData(new TableTerm { Name = null, Alias = String.Empty })
                       .SetName("A table term where name is set to null and alias is String.Empty is not valid")
                       .Returns(false);
                }
            }

        }

        [SetUp]
        public void SetUp()
        {
            _validator = new TableValidator();
        }

        [TearDown]
        public void TearDown()
        {
            _validator = null;
        }

        [TestCaseSource(typeof(Cases), "ValidateTableTermTestCases")]
        public bool ValidateSelectQuery(TableTerm selectQuery)
        {
            return _validator.Validate(selectQuery);
        }
    }
}