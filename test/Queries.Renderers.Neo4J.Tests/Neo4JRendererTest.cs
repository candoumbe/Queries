using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Extensions;
using Xunit;

using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Renderers.Neo4J.Tests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Neo4JRendererTest
    {
        public static IEnumerable<object[]> SelectTestCases
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


            }
        }


        public static IEnumerable<object[]> InsertTestCases
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
            }
        }

        [Theory]
        [MemberData(nameof(SelectTestCases))]
        //[TestCaseSource(typeof(Cases), nameof(Cases.SelectTestCases))]
        public void SelectTest(SelectQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);

        [Theory]
        [MemberData(nameof(InsertTestCases))]
        //[TestCaseSource(typeof(Cases), nameof(Cases.SelectTestCases))]
        public void InsertTest(InsertIntoQuery query, bool prettyPrint, string expectedString)
            => IsQueryOk(query, prettyPrint, expectedString);



        private static void IsQueryOk(IQuery query, bool prettyPrint, string expectedString) => Assert.Equal(expectedString, query.ForNeo4J(prettyPrint));
    }



}