﻿using FluentAssertions;
using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Renderers.Sqlite
{
    [UnitTest]
    [Feature("Sqlite")]
    public class ReplaceParameterBySelectQueryVisitorTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public ReplaceParameterBySelectQueryVisitorTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        [Fact]
        public void CanVisitQuery() => typeof(ReplaceParameterBySelectQueryVisitor).Should()
            .NotBeAbstract().And
            .NotHaveDefaultConstructor().And
            .Implement<IVisitor<IQuery>>();

        [Fact]
        public void Throws_ArgumentNullException_When_Param_IsNull()
        {
            // Act
            Action action = () => new ReplaceParameterBySelectQueryVisitor(null);

            // Assert
            action.Should()
                .ThrowExactly<ArgumentNullException>()
                .Where(ex => !string.IsNullOrWhiteSpace(ex.ParamName));
        }

        public static IEnumerable<object[]> ReplaceParameterBySelectQueryVisitorCases
        {
            get
            {
                yield return new object[]
                {
                    Select("*").From("superHeroes")
                        .Where("Fullname".Field().Like("Bat%")),
                    (Func<Variable, SelectQuery>)(param => Select(Null("RealValue".Field(), "TextValue".Field())).Limit(1)
                        .From("Parameter")
                        .Where("ParameterName".Field().EqualTo(param.Name)).Build()),
                    Select("*")
                        .From("superHeroes")
                        .Where("Fullname".Field()
                            .Like(
                                Select(Null("RealValue".Field(), "TextValue".Field()))
                                .Limit(1)
                                .From("Parameter")
                                .Where("ParameterName".Field().EqualTo("p0"))
                                .Build()
                            )
                        )
                };
            }
        }

        [Theory]
        [MemberData(nameof(ReplaceParameterBySelectQueryVisitorCases))]
        public void RewriteQuery(IQuery initialQuery, Func<Variable, SelectQuery> rewriter, IQuery expected)
        {
            // Arrange
            ReplaceParameterBySelectQueryVisitor visitor = new ReplaceParameterBySelectQueryVisitor(rewriter);

            // Act
            visitor.Visit(initialQuery);

            // Assert
            initialQuery.Should()
                .Be(expected);
        }
    }
}
