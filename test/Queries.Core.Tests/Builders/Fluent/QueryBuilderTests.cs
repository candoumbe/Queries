using FluentAssertions;
using Queries.Core.Builders;
using Queries.Core.Builders.Fluent;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Builders.Fluent
{
    public class QueryBuilderTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public QueryBuilderTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public void Dispose() => _outputHelper = null;

        public static IEnumerable<object[]> SelectQueryFluentCases
        {
            get
            {
                yield return new object[]
                {
                    Select(1.Literal()),
                    ((Expression<Func<SelectQuery, bool>>)(query => new [] { 1.Literal() }.SequenceEqual(query.Columns)
                        && query.Tables.Count == 0
                        && query.WhereCriteria == null
                        && query.Unions.Count == 0
                        && query.Sorts.Count == 0
                    )),
                };

                yield return new object[]
                {
                    Select("Firstname", "Lastname").From("SuperHero"),
                    ((Expression<Func<SelectQuery, bool>>)(query =>
                        new [] { "Firstname".Field(), "Lastname".Field() }.SequenceEqual(query.Columns)
                        && new [] { "SuperHero".Table(null) }.SequenceEqual(query.Tables)
                        && query.WhereCriteria == null
                        && query.Unions.Count == 0 
                        && query.Sorts.Count == 0
                    )),
                };

                yield return new object[]
                {
                    Select(Concat("Firstname".Field(), "Lastname".Field())).From("SuperHero"),
                    ((Expression<Func<SelectQuery, bool>>)(query =>
                        new [] { Concat("Firstname".Field(), "Lastname".Field()) }.SequenceEqual(query.Columns)
                        && new [] { "SuperHero".Table(null) }.SequenceEqual(query.Tables)
                        && query.WhereCriteria == null
                        && query.Unions.Count == 0
                        && query.Sorts.Count == 0
                    )),
                };
            }
        }


        [Theory]
        [MemberData(nameof(SelectQueryFluentCases))]
        public void SelectQueryBuildTests(IBuildableQuery<SelectQuery> queryBuilder, Expression<Func<SelectQuery, bool>> queryExpectation)
            => BuildTests(queryBuilder, queryExpectation);



        private void BuildTests<T>(IBuildableQuery<T> queryBuilder, Expression<Func<T, bool>> queryExpectation)
        {
            _outputHelper.WriteLine($"{nameof(queryBuilder)} : {queryBuilder}");

            // Act
            T query = queryBuilder.Build();

            // Assert
            query.Should().Match(queryExpectation);
        }


    }
}
