using FluentAssertions;

using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;

using System;
using System.Collections.Generic;

using Xunit;
using Xunit.Abstractions;

using static Queries.Core.Builders.Fluent.QueryBuilder;
using static Queries.Core.Parts.Clauses.ClauseOperator;

namespace Queries.Renderers.Neo4J.Tests;

// This project can output the Class library as a NuGet Package.
// To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
public class Neo4JRendererTest
{
<<<<<<< HEAD
    private ITestOutputHelper _output;

    public static IEnumerable<object[]> SelectCases
    {
        get
        {
            yield return new object[]
            {
                Select("*").From("Hero".Table("h")),
                new Neo4JRendererSettings{ PrettyPrint = false },
                "MATCH (h:Hero) RETURN h;"
            };

            yield return new object[]
=======
    public class Neo4JRendererTest
    {
        private readonly ITestOutputHelper _output;

        public Neo4JRendererTest(ITestOutputHelper output) => _output = output;

        public static IEnumerable<object[]> InsertCases
        {
            get
>>>>>>> c2bba33 (feat(renderer) : improve pretty print)
            {
                Select("*")
                    .From("Hero".Table("h"))
                    .Where(new WhereClause("Firstname".Field(), EqualTo, "Wayne"))
                ,
                new Neo4JRendererSettings{ PrettyPrint = false },
                "MATCH (h:Hero) WHERE (Firstname = 'Wayne') RETURN h;"
            };

            yield return new object[]
            {
                Select("*").From("Hero".Table("h")),
                new Neo4JRendererSettings{ PrettyPrint = true },
                $"MATCH (h:Hero) {Environment.NewLine}" +
                "RETURN h;"
            };

<<<<<<< HEAD
            yield return new object[]
=======
        public static IEnumerable<object[]> BatchQueriesCases
        {
            get
>>>>>>> c2bba33 (feat(renderer) : improve pretty print)
            {
                Select("*").From("Hero"),
                new Neo4JRendererSettings{ PrettyPrint = false },
                "MATCH (h:Hero) RETURN h;"
            };

<<<<<<< HEAD
            yield return new object[]
            {
                Select("h1", "h2")
                    .From("Heroe".Table("h1"), "Heroe".Table("h2"))
                    .Where(new CompositeWhereClause {
                        Logic = ClauseLogic.And,
                        Clauses = new [] {
                            new WhereClause("h1.Lastname".Field(), EqualTo, "Wayne"),
                            new WhereClause("h2.Lastname".Field(), EqualTo, "Kent")
                        }
                    }),
                new Neo4JRendererSettings{ PrettyPrint = true },
                $"MATCH (h1:Heroe), (h2:Heroe) {Environment.NewLine}" +
                $"WHERE ((h1.Lastname = 'Wayne') AND (h2.Lastname = 'Kent')) {Environment.NewLine}" +
                "RETURN h1, h2;"
            };
=======
                            Select("*").From("Heroe").Where("Firstname".Field(), EqualTo, "Bruce"),

                            InsertInto("Disease")
                                .Values(
                                    "Code".InsertValue("Batman".Literal()),
                                    "Name".InsertValue("Lack of humanity".Literal())
                                )
                        ),
                        true,
                    }
                };
            }
        }

        public static IEnumerable<object[]> SelectCases
        {
            get
            {
                yield return new object[]
                {
                    Select("*").From("Hero".Table("h")),
                    new Neo4JRendererSettings{ PrettyPrint = false },
                    "MATCH (h:Hero) RETURN h;"
                };

                yield return new object[]
                {
                    Select("*")
                        .From("Hero".Table("h"))
                        .Where(new WhereClause("Firstname".Field(), EqualTo, "Wayne"))
                    ,
                    new Neo4JRendererSettings{ PrettyPrint = false },
                    "MATCH (h:Hero) WHERE (Firstname = 'Wayne') RETURN h;"
                };

                yield return new object[]
                {
                    Select("*").From("Hero".Table("h")),
                    new Neo4JRendererSettings{ PrettyPrint = true },
                    $"MATCH (h:Hero){Environment.NewLine}" +
                    "RETURN h;"
                };

                yield return new object[]
                {
                    Select("*").From("Hero"),
                    new Neo4JRendererSettings{ PrettyPrint = false },
                    "MATCH (h:Hero) RETURN h;"
                };
            }
        }

        [Theory]
        [MemberData(nameof(SelectCases))]
        public void SelectTest(SelectQuery query, Neo4JRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        [Theory]
        [MemberData(nameof(InsertCases))]
        public void InsertTest(InsertIntoQuery query, Neo4JRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        [Theory]
        [MemberData(nameof(DeleteCases))]
        public void DeleteTest(DeleteQuery query, Neo4JRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        private void IsQueryOk(IQuery query, Neo4JRendererSettings settings, string expectedString)
        {
            _output.WriteLine(
                $"Building : {query.Jsonify()}{Environment.NewLine}" +
                $"{nameof(settings)} : {settings.Jsonify()}");
            query.ForNeo4J(settings).Should().Be(expectedString);
>>>>>>> c2bba33 (feat(renderer) : improve pretty print)
        }
    }

    public static IEnumerable<object[]> InsertCases
    {
        get
        {
            yield return new object[]
            {
                InsertInto("Hero")
                    .Values(
                        "firstname".InsertValue("Bruce".Literal()),
                        "lastname".InsertValue("Wayne".Literal()),
                        "nickname".InsertValue("Batman".Literal())),
                new Neo4JRendererSettings{ PrettyPrint = false },
                "CREATE (h:Hero {firstname : 'Bruce', lastname : 'Wayne', nickname : 'Batman'})"
            };

            yield return new object[]
            {
                InsertInto("Hero")
                    .Values(
                        "firstname".InsertValue("Bruce".Literal()),
                        "lastname".InsertValue("Wayne".Literal()),
                        "nickname".InsertValue("Batman".Literal()),
                        "superpowers".InsertValue(null)),
                new Neo4JRendererSettings{ PrettyPrint = false },
                "CREATE (h:Hero {firstname : 'Bruce', lastname : 'Wayne', nickname : 'Batman', superpowers : NULL})"
            };

            yield return new object[]
            {
                InsertInto("Hero")
                    .Values(
                        "firstname".InsertValue("Bruce".Literal()),
                        "lastname".InsertValue("Wayne".Literal()),
                        "nickname".InsertValue(Upper("Batman".Literal()))),
                new Neo4JRendererSettings{ PrettyPrint = false },
                "CREATE (h:Hero {firstname : 'Bruce', lastname : 'Wayne', nickname : UPPER('Batman')})"
            };
        }
    }

    public static IEnumerable<object[]> DeleteCases
    {
        get
        {
            yield return new object[]
            {
                Delete("Heroes").Where("Firstname".Field(), EqualTo, "Wayne"),
                new Neo4JRendererSettings{ PrettyPrint = false },
                "MATCH (h:Heroes) WHERE (Firstname = 'Wayne') DELETE h"
            };
        }
    }

    public IEnumerable<object[]> BatchQueriesCases
    {
        get
        {
            yield return new object[]
            {
                new object[] {
                    new BatchQuery(

                        Select("*").From("Heroe").Where("Firstname".Field(), EqualTo, "Bruce"),

                        InsertInto("Disease")
                            .Values(
                                "Code".InsertValue("Batman".Literal()),
                                "Name".InsertValue("Lack of humanity".Literal())
                            )
                    ),
                    true,
                }
            };
        }
    }

    public Neo4JRendererTest(ITestOutputHelper output) => _output = output;

    [Theory]
    [MemberData(nameof(SelectCases))]
    public void SelectTest(SelectQuery query, Neo4JRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    [Theory]
    [MemberData(nameof(InsertCases))]
    public void InsertTest(InsertIntoQuery query, Neo4JRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    [Theory]
    [MemberData(nameof(DeleteCases))]
    public void DeleteTest(DeleteQuery query, Neo4JRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    private void IsQueryOk(IQuery query, Neo4JRendererSettings settings, string expectedString)
    {
        _output.WriteLine(
            $"Building : {SerializeObject(query, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented })}{Environment.NewLine}" +
            $"{nameof(settings)} : {SerializeObject(settings)}");
        query.ForNeo4J(settings).Should().Be(expectedString);
    }
}