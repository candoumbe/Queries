using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Queries.Core.Builders;
using Queries.Core.Builders.Fluent;
using Queries.Core.Parts;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Builders.Fluent;

[UnitTest]
[Feature("Builder")]
public class QueryBuilderTests(ITestOutputHelper outputHelper)
{
    public static TheoryData<IBuild<SelectQuery>, Expression<Func<SelectQuery, bool>>> SelectQueryFluentCases = new()
    {
        {
            Select(1.Literal()),
            query => query.Columns.Cast<NumericColumn>().SequenceEqual(new [] { 1.Literal() })
                     && query.Tables.Count == 0
                     && query.WhereCriteria == null
                     && query.Unions.Count == 0
                     && query.Orders.Count == 0
        },
        {
            Select("Firstname", "Lastname").From("SuperHero"),
            query =>
                new [] { "Firstname".Field(), "Lastname".Field() }.SequenceEqual(query.Columns)
                && new [] { "SuperHero".Table(null) }.SequenceEqual(query.Tables)
                && query.WhereCriteria == null
                && query.Unions.Count == 0
                && query.Orders.Count == 0
        },
        {
            Select(Concat("Firstname".Field(), "Lastname".Field())).From("SuperHero"),
            query =>
                new [] { Concat("Firstname".Field(), "Lastname".Field()) }.SequenceEqual(query.Columns)
                && new [] { "SuperHero".Table(null) }.SequenceEqual(query.Tables)
                && query.WhereCriteria == null
                && query.Unions.Count == 0
                && query.Orders.Count == 0
        }
    };

    [Feature("Select")]
    [Theory]
    [MemberData(nameof(SelectQueryFluentCases))]
    public void SelectQueryBuildTests(IBuild<SelectQuery> queryBuilder, Expression<Func<SelectQuery, bool>> queryExpectation)
        => BuildTests(queryBuilder, queryExpectation);

    public static TheoryData<IBuild<Variable>, Expression<Func<Variable, bool>>> DeclareVariableFluentCases = new()
    {
        {
            Declare("p").WithValue(3).Numeric(),
            variable =>
                variable.Name == "p"
                && variable.Type == VariableType.Numeric
                && 3.Equals((int)variable.Value)
        },
        {
            Declare("p").WithValue("Noname").String(),
            variable =>
                variable.Name == "p"
                && variable.Type == VariableType.String
                && "Noname".Equals((string)variable.Value)
        },
        {
            Declare("p").Numeric(),
            variable =>
                variable.Name == "p"
                && variable.Type == VariableType.Numeric
                && variable.Value == null
        },
        {
            Declare("p").Date(),
            variable =>
                variable.Name == "p"
                && variable.Type == VariableType.Date
                && variable.Value == null
        }
    };

    [Theory]
    [MemberData(nameof(DeclareVariableFluentCases))]
    public void DeclareVariableBuildTests(IBuild<Variable> queryBuilder, Expression<Func<Variable, bool>> queryExpectation)
        => BuildTests(queryBuilder, queryExpectation);

    private void BuildTests<T>(IBuild<T> queryBuilder, Expression<Func<T, bool>> queryExpectation)
    {
        outputHelper.WriteLine($"{nameof(queryBuilder)} : {queryBuilder}");

        // Act
        T query = queryBuilder.Build();

        // Assert
        query.Should().Match(queryExpectation);
    }
}