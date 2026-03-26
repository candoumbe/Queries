using Xunit;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using System.Collections.Generic;
using Queries.Core;
using Queries.Core.Builders;
using AwesomeAssertions;
using System;
using Queries.Core.Renderers;

namespace Queries.Renderers.MySQL.Tests;

// This project can output the Class library as a NuGet Package.
// To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
public class MySqlRendererTest
{
    public static TheoryData<SelectQuery, MySqlRendererSettings, string> SelectTestCases
        => new()
        {
            {
                Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field())),
                new MySqlRendererSettings { PrettyPrint = false },
                @"SELECT CONCAT(""firstname"", ' ', ""lastname"")"
            },
            {
                new SelectQuery(Concat("firstname".Field(), " ".Literal(), "lastname".Field())),
                new MySqlRendererSettings { PrettyPrint = false },
                @"SELECT CONCAT(""firstname"", ' ', ""lastname"")"
            }
        };

    [Theory]
    [MemberData(nameof(SelectTestCases))]
    public void SelectTest(SelectQuery query, QueryRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    private static void IsQueryOk(IQuery query, QueryRendererSettings settings, string expectedString) =>
        query.ForMySql(settings).Should().Be(expectedString);
}