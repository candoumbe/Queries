using System;
using FsCheck;
using FsCheck.Xunit;
using Xunit.Categories;
using Queries.EntityFrameworkCore.Extensions.Operations;
using Queries.Core.Builders;
using TestsHelpers;

namespace Queries.EntityFrameworkCore.Extensions.Tests.Operations
{
    [UnitTest]
    public class DeleteMigrationOperationTests
    {
        [Property]
        public Property Ctor_throws_ArgumentNullException_when_query_is_null(string schema)
        {
            // Act
            Lazy<DeleteMigrationOperation> ctorWithNullLazy = new(() => new DeleteMigrationOperation(null, schema));

            // Assert
            return Prop.Throws<ArgumentNullException, DeleteMigrationOperation>(ctorWithNullLazy);
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
}