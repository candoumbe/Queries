using Xunit;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using System.Collections.Generic;
using Queries.Core;
using Queries.Core.Builders;
using FluentAssertions;
using System;
using Queries.Core.Renderers;

namespace Queries.Renderers.MySQL.Tests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class MySQLRendererTest
    {
        public static IEnumerable<object[]> SelectTestCases
        {
            get
            {
                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field())),
                    new QueryRendererSettings { PrettyPrint = false },
                    @"SELECT CONCAT(""firstname"", ' ', ""lastname"")"
                };

                yield return new object[]
                {
                    new SelectQuery(Concat("firstname".Field(), " ".Literal(), "lastname".Field())),
                    new QueryRendererSettings { PrettyPrint = false },
                    @"SELECT CONCAT(""firstname"", ' ', ""lastname"")"
                };
            }
        }

        [Theory]
        [MemberData(nameof(SelectTestCases))]
        //[TestCaseSource(typeof(Cases), nameof(Cases.SelectTestCases))]
        public void SelectTest(SelectQuery query, QueryRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        private static void IsQueryOk(IQuery query, QueryRendererSettings settings, string expectedString) =>
            query.ForMySQL(settings).Should().Be(expectedString);
    }
}
