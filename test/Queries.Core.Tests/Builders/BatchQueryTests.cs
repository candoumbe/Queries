﻿using FluentAssertions;

using Queries.Core.Builders;

using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;
using Xunit.Categories;

using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Builders;

[UnitTest]
[Feature("Batch query")]
[Feature("Builder")]
public class BatchQueryTests
{
    [Fact]
    public void CtorWithNoArgumentBuildsEmptyInstance()
    {
        // Act
        BatchQuery batch = new();

        // Assert
        batch.Statements.Should()
            .BeEmpty();
    }

    [Fact]
    public void CtorShouldExcludeNullStatements()
    {
        // Arrange
        IEnumerable<IQuery> queries = new IQuery[]
        {
            InsertInto("SuperHero")
                .Values(Select("firstname", "lastname", "nickname").From("DC_SuperHero").Build()),

            null,

            Delete("DC_SuperHero")

        };

        // Act
        BatchQuery batchQuery = new(queries.ToArray());

        // Assert
        batchQuery.Statements.Should()
            .HaveCount(2).And
            .NotContainNulls();
    }

    [Fact]
    public void CtorPreserveStatementOrder()
    {
        // Arrange
        IEnumerable<IQuery> queries = new IQuery[]
        {
            Select(1.Literal()),
            Select(2.Literal()),
            Select(3.Literal())
        };

        // Act
        BatchQuery batchQuery = new(queries.ToArray());

        // Assert 
        batchQuery.Statements.Should()
            .Equal(new[]
            {
                Select(1.Literal()),
                Select(2.Literal()),
                Select(3.Literal())

            });
    }
}
