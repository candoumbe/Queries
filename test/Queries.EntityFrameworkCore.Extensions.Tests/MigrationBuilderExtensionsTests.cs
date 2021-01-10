using System;
using System.Collections.Generic;
using FsCheck;
using FsCheck.Xunit;
using Microsoft.EntityFrameworkCore.Migrations;
using Queries.Core.Builders;
using Queries.EntityFrameworkCore.Extensions.Operations;
using Queries.EntityFrameworkCore.Extensions.Tests.Helpers;
using Xunit.Categories;

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
    }
}
