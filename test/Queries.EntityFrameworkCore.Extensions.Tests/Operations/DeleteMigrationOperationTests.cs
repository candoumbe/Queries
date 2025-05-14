using System;
using FluentAssertions;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Xunit.Categories;
using Queries.EntityFrameworkCore.Extensions.Operations;
using Queries.Core.Builders;
using TestsHelpers;

namespace Queries.EntityFrameworkCore.Extensions.Tests.Operations;

[UnitTest]
public class DeleteMigrationOperationTests
{
    [Property]
    public void Ctor_throws_ArgumentNullException_when_query_is_null(string schema)
    {
        // Act
        Action ctorWithNullLazy = () => _ = new DeleteMigrationOperation(null, schema);

        // Assert
        ctorWithNullLazy.Should().ThrowExactly<ArgumentNullException>();
    }

    [Property(Arbitrary = new[] { typeof(QueryGenerators) })]
    public Property Ctor_populates_properties(DeleteQuery query, string schema)
    {
        DeleteMigrationOperation operation = new(query, schema);

        return operation.Query.Equals(query).Label("Query")
            .And(operation.Schema == schema).Label("Schema")
            .And(operation.IsDestructiveChange).Label(nameof(operation.IsDestructiveChange));
    }
}