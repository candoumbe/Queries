using FluentAssertions;

using Newtonsoft.Json;

using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Queries.Core.Builders.Fluent;
using Xunit;
using Xunit.Abstractions;

using static Newtonsoft.Json.JsonConvert;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using static Queries.Core.Parts.Clauses.ClauseOperator;


namespace Queries.Renderers.Neo4J.Tests;

// This project can output the Class library as a NuGet Package.
// To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
public class Neo4JRendererTest(ITestOutputHelper output)
{
    public TheoryData<BatchQuery, bool> BatchQueriesCases
     => new()
        {
            {
                new BatchQuery(

                    Select("*").From("Hero").Where("Firstname".Field(), EqualTo, "Bruce"),

                    InsertInto("Disease")
                        .Values(
                            "Code".InsertValue("Batman".Literal()),
                            "Name".InsertValue("Lack of humanity".Literal())
                        )
                ),
                true
            }
        };

    public static TheoryData<IBuild<SelectQuery>, Neo4JRendererSettings, string> SelectCases
        => new()
        {
            {
                Select("*").From("Hero".Table("h")),
                new Neo4JRendererSettings{ PrettyPrint = false },
                "MATCH (h:Hero) RETURN h;"
            },
            {
                Select("*")
                    .From("Hero".Table("h"))
                    .Where(new WhereClause("Firstname".Field(), EqualTo, "Wayne"))
                ,
                new Neo4JRendererSettings{ PrettyPrint = false },
                "MATCH (h:Hero) WHERE (Firstname = 'Wayne') RETURN h;"
            },
            {
                Select("*").From("Hero".Table("h")),
                new Neo4JRendererSettings{ PrettyPrint = true },
                $"MATCH{Environment.NewLine}" +
                $"    (h:Hero){Environment.NewLine}" +
                $"RETURN{Environment.NewLine}" +
                "    h;"
            },
            {
                Select("*").From("Hero"),
                new Neo4JRendererSettings{ PrettyPrint = false },
                "MATCH (h:Hero) RETURN h;"
            },
            {
                Select("h1", "h2")
                    .From("Hero".Table("h1"), "Hero".Table("h2"))
                    .Where(new CompositeWhereClause {
                        Logic = ClauseLogic.And,
                        Clauses =
                        [
                            new WhereClause("h1.Lastname".Field(), EqualTo, "Wayne"),
                            new WhereClause("h2.Lastname".Field(), EqualTo, "Kent")
                        ]
                    }),
                new Neo4JRendererSettings{ PrettyPrint = false },
                "MATCH (h1:Hero), (h2:Hero) " +
                "WHERE ((h1.Lastname = 'Wayne') AND (h2.Lastname = 'Kent')) " +
                "RETURN h1, h2;"
        }
    };

    [Theory]
    [MemberData(nameof(SelectCases))]
    public void SelectTest(SelectQuery query, Neo4JRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    public static TheoryData<IBuild<InsertIntoQuery>, Neo4JRendererSettings, string> InsertCases
        => new ()
        {
            {
                InsertInto("Hero")
                    .Values(
                        "firstname".InsertValue("Bruce".Literal()),
                        "lastname".InsertValue("Wayne".Literal()),
                        "nickname".InsertValue("Batman".Literal())),
                new Neo4JRendererSettings{ PrettyPrint = false },
                "CREATE (h:Hero {firstname : 'Bruce', lastname : 'Wayne', nickname : 'Batman'})"
            },
            {
                InsertInto("Hero")
                    .Values(
                        "firstname".InsertValue("Bruce".Literal()),
                        "lastname".InsertValue("Wayne".Literal()),
                        "nickname".InsertValue("Batman".Literal()),
                        "superpowers".InsertValue(null)),
                new Neo4JRendererSettings{ PrettyPrint = false },
                "CREATE (h:Hero {firstname : 'Bruce', lastname : 'Wayne', nickname : 'Batman', superpowers : NULL})"
            },
            {
                InsertInto("Hero")
                    .Values(
                        "firstname".InsertValue("Bruce".Literal()),
                        "lastname".InsertValue("Wayne".Literal()),
                        "nickname".InsertValue(Upper("Batman".Literal()))),
                new Neo4JRendererSettings{ PrettyPrint = false },
                "CREATE (h:Hero {firstname : 'Bruce', lastname : 'Wayne', nickname : UPPER('Batman')})"
            }
        };

    [Theory]
    [MemberData(nameof(InsertCases))]
    public void InsertTest(InsertIntoQuery query, Neo4JRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    public static TheoryData<IBuild<DeleteQuery>, Neo4JRendererSettings, string> DeleteCases
        => new ()
        {
            {
                Delete("Heroes").Where("Firstname".Field(), EqualTo, "Wayne"),
                new Neo4JRendererSettings{ PrettyPrint = false },
                "MATCH (h:Heroes) WHERE (Firstname = 'Wayne') DELETE h"
            }
        };

    [Theory]
    [MemberData(nameof(DeleteCases))]
    public void DeleteTest(DeleteQuery query, Neo4JRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    private void IsQueryOk(IQuery query, Neo4JRendererSettings settings, string expectedString)
    {
        output.WriteLine(
            $"Building : {query.Jsonify(new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, WriteIndented = true })}){Environment.NewLine}" +
            $"{nameof(settings)} : {SerializeObject(settings)}");
        query.ForNeo4J(settings).Should().Be(expectedString);
    }
}