using System;
using FsCheck;
using FsCheck.Xunit;
using Xunit.Categories;
using Queries.EntityFrameworkCore.Extensions.Operations;
using Queries.Core.Builders;
using Queries.EntityFrameworkCore.Extensions.Tests.Helpers;

namespace Queries.EntityFrameworkCore.Extensions.Tests.Operations
{
    [UnitTest]
    public class CreateViewMigrationOperationTests
    {
        [Property]
        public Property Ctor_throws_ArgumentNullException_when_query_is_null(string schema)
        {
            // Act
            Lazy<CreateViewMigrationOperation> ctorWithNullLazy = new(() => new CreateViewMigrationOperation(null, schema));

            // Assert
            return Prop.Throws<ArgumentNullException, CreateViewMigrationOperation>(ctorWithNullLazy);
        }

        [Property(Arbitrary = new[] { typeof(QueryGenerators) })]
        public Property Ctor_populates_properties(CreateViewQuery query, string schema)
        {
            CreateViewMigrationOperation operation = new(query, schema);

            return operation.Query.Equals(query).Label("Query")
                .And(operation.Schema == schema).Label("Schema")
                .And(!operation.IsDestructiveChange).Label(nameof(operation.IsDestructiveChange));
        }
    }
}