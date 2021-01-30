using System;
using System.Collections.Generic;
using FsCheck;
using FsCheck.Xunit;
using Microsoft.EntityFrameworkCore.Migrations;
using Queries.Core.Builders;
using Queries.EntityFrameworkCore.Extensions.Operations;
using Xunit.Categories;
using TestsHelpers;

namespace Queries.EntityFrameworkCore.Extensions.Tests
{
    [UnitTest]
    public class MigrationBuilderExtensionsTests
    {
        [Property(Arbitrary = new[] { typeof(QueryGenerators) })]
        public Property CreateView(NonWhiteSpaceString providerName, CreateViewQuery createView, string schema)
        {
            // Arrange
            Lazy<MigrationBuilder> createViewLazy = new(() =>
            {
                MigrationBuilder builder = new(providerName.Item);
                return builder.CreateView(createView, schema);
            });

            // Act
            return Prop.Throws<ArgumentNullException, MigrationBuilder>(createViewLazy).Label("ArgumentNullCases")
                       .When(createView is null)
                       .Or(createViewLazy.Value.Operations.Once(op => op is CreateViewMigrationOperation).ToProperty().Label("Builder must have the corresponding operation")).When(createView is not null);
        }

        [Property(Arbitrary = new[] { typeof(QueryGenerators) })]
        public Property Delete(NonWhiteSpaceString providerName, DeleteQuery deleteQuery, string schema)
        {
            // Arrange
            Lazy<MigrationBuilder> deleteQueryLazy = new(() =>
            {
                MigrationBuilder builder = new(providerName.Item);
                return builder.Delete(deleteQuery, schema);
            });

            // Act
            return Prop.Throws<ArgumentNullException, MigrationBuilder>(deleteQueryLazy).Label("ArgumentNullCases")
                       .When(deleteQuery is null)
                       .Or(deleteQueryLazy.Value.Operations.Once(op => op is DeleteMigrationOperation)).When(deleteQuery is not null)
                                                                                                       .Label("Builder must have the corresponding operation");
        }
    }
}
