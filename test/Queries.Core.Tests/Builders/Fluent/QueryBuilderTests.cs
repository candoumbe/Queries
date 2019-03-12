using FluentAssertions;
using Queries.Core.Builders;
using Queries.Core.Builders.Fluent;
using Queries.Core.Parts.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Builders.Fluent
{
    [UnitTest]
    [Feature("Builder")]
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
                    (Expression<Func<SelectQuery, bool>>)(query => new [] { 1.Literal() }.SequenceEqual(query.Columns)
                        && query.Tables.Count == 0
                        && query.WhereCriteria == null
                        && query.Unions.Count == 0
                        && query.Sorts.Count == 0
                    ),
                };

                yield return new object[]
                {
                    Select("Firstname", "Lastname").From("SuperHero"),
                    (Expression<Func<SelectQuery, bool>>)(query =>
                        new [] { "Firstname".Field(), "Lastname".Field() }.SequenceEqual(query.Columns)
                        && new [] { "SuperHero".Table(null) }.SequenceEqual(query.Tables)
                        && query.WhereCriteria == null
                        && query.Unions.Count == 0
                        && query.Sorts.Count == 0
                    ),
                };

                yield return new object[]
                {
                    Select(Concat("Firstname".Field(), "Lastname".Field())).From("SuperHero"),
                    (Expression<Func<SelectQuery, bool>>)(query =>
                        new [] { Concat("Firstname".Field(), "Lastname".Field()) }.SequenceEqual(query.Columns)
                        && new [] { "SuperHero".Table(null) }.SequenceEqual(query.Tables)
                        && query.WhereCriteria == null
                        && query.Unions.Count == 0
                        && query.Sorts.Count == 0
                    ),
                };
            }
        }

        [Feature("Select")]
        [Theory]
        [MemberData(nameof(SelectQueryFluentCases))]
        public void SelectQueryBuildTests(IBuild<SelectQuery> queryBuilder, Expression<Func<SelectQuery, bool>> queryExpectation)
            => BuildTests(queryBuilder, queryExpectation);

        public static IEnumerable<object[]> DeclareVariableFluentCases
        {
            get
            {
                yield return new object[]
                {
                    Declare("p").WithValue(3).Numeric(),
                    (Expression<Func<Variable, bool>>)(variable =>
                        variable.Name == "p"
                        && variable.Type == VariableType.Numeric
                        && 3.Equals(variable.Value)
                    ),
                };

                yield return new object[]
                {
                    Declare("p").WithValue("Noname").String(),
                    (Expression<Func<Variable, bool>>)(variable =>
                        variable.Name == "p"
                        && variable.Type == VariableType.String
                        && "Noname".Equals(variable.Value)
                    ),
                };

                yield return new object[]
                {
                    Declare("p").Numeric(),
                    (Expression<Func<Variable, bool>>)(variable =>
                        variable.Name == "p"
                        && variable.Type == VariableType.Numeric
                        && variable.Value == null
                    ),
                };

                yield return new object[]
                {
                    Declare("p").Date(),
                    (Expression<Func<Variable, bool>>)(variable =>
                        variable.Name == "p"
                        && variable.Type == VariableType.Date
                        && variable.Value == null
                    ),
                };
            }
        }

        [Theory]
        [MemberData(nameof(DeclareVariableFluentCases))]
        public void DeclareVariableBuildTests(IBuild<Variable> queryBuilder, Expression<Func<Variable, bool>> queryExpectation)
            => BuildTests(queryBuilder, queryExpectation);

        private void BuildTests<T>(IBuild<T> queryBuilder, Expression<Func<T, bool>> queryExpectation)
        {
            _outputHelper.WriteLine($"{nameof(queryBuilder)} : {queryBuilder}");

            // Act
            T query = queryBuilder.Build();

            // Assert
            query.Should().Match(queryExpectation);
        }
    }
}
