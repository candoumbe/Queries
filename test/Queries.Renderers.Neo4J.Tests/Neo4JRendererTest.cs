using System;
using System.Collections.Generic;
using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Extensions;
using Xunit;

using static Queries.Core.Parts.Clauses.ClauseOperator;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using Xunit.Abstractions;
using FluentAssertions;
using Newtonsoft.Json;
using Queries.Core.Parts.Clauses;

namespace Queries.Renderers.Neo4J.Tests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Neo4JRendererTest
    {
        private readonly ITestOutputHelper _output;

        public static IEnumerable<object[]> SelectCases
        {
            get
            {
                yield return new object[]
                {
                    Select("*").From("Hero".Table("h")),
                    false,
                    "MATCH (h:Hero) RETURN h;"
                };

                yield return new object[]
                {
                    Select("*")
                        .From("Hero".Table("h"))
                        .Where(new WhereClause("Firstname".Field(), EqualTo, "Wayne"))
                    ,
                    false,
                    "MATCH (h:Hero) WHERE (Firstname = 'Wayne') RETURN h;"
                };

                yield return new object[]
                {
                    Select("*").From("Hero".Table("h")),
                    true,
                    $"MATCH (h:Hero) {Environment.NewLine}" +
                    "RETURN h;"
                };


                yield return new object[]
                {
                    Select("*").From("Hero"),
                    false,
                    "MATCH (h:Hero) RETURN h;"
                };


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
                    true,
                    $"MATCH (h1:Heroe), (h2:Heroe) {Environment.NewLine}" +
                    $"WHERE ((h1.Lastname = 'Wayne') AND (h2.Lastname = 'Kent')) {Environment.NewLine}" +
                    "RETURN h1, h2;"
                };
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
                    false,
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
                    false,
                    "CREATE (h:Hero {firstname : 'Bruce', lastname : 'Wayne', nickname : 'Batman', superpowers : NULL})"
                };


                yield return new object[]
                {
                    InsertInto("Hero")
                        .Values(
                            "firstname".InsertValue("Bruce".Literal()),
                            "lastname".InsertValue("Wayne".Literal()),
                            "nickname".InsertValue(Upper("Batman".Literal()))),
                    false,
                    @"CREATE (h:Hero {firstname : 'Bruce', lastname : 'Wayne', nickname : UPPER('Batman')})"
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
                    false,
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
                                    "Code".InsertValue("LOH".Literal()),
                                    "Name".InsertValue("Lack of humanity".Literal())
                                )
                        ),
                        true,

                    }
                };
            }
        }

        public Neo4JRendererTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [MemberData(nameof(SelectCases))]
        //[TestCaseSource(typeof(Cases), nameof(Cases.SelectTestCases))]
        public void SelectTest(SelectQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);

        [Theory]
        [MemberData(nameof(InsertCases))]
        //[TestCaseSource(typeof(Cases), nameof(Cases.SelectTestCases))]
        public void InsertTest(InsertIntoQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);


        [Theory]
        [MemberData(nameof(DeleteCases))]
        //[TestCaseSource(typeof(Cases), nameof(Cases.SelectTestCases))]
        public void DeleteTest(DeleteQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);


        private void IsQueryOk(IQuery query, bool prettyPrint, string expectedString)
        {
            _output.WriteLine(
                $"Building : {JsonConvert.SerializeObject(query, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented })}{Environment.NewLine}" +
                $"Pretting print : {prettyPrint}");
            query.ForNeo4J(prettyPrint).Should().Be(expectedString);
        }
    }



}