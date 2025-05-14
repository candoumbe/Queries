using System;
using System.Collections.Generic;
using FluentAssertions;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Microsoft.EntityFrameworkCore.Migrations;
using Queries.Core.Builders;
using Queries.EntityFrameworkCore.Extensions.Operations;
using Xunit.Categories;
using TestsHelpers;

namespace Queries.EntityFrameworkCore.Extensions.Tests;

[UnitTest]
public class MigrationBuilderExtensionsTests
{
    [Property(Arbitrary = [typeof(QueryGenerators)])]
    public void CreateView(NonWhiteSpaceString providerName, CreateViewQuery createView, string schema)
    {
        // Arrange
        Func<MigrationBuilder> createViewLazy = () =>
        {
            MigrationBuilder builder = new(providerName.Item);
            return builder.CreateView(createView, schema);
        };

        // Assert
        object _ = (createView is null) switch
        {
            true => createViewLazy.Should().ThrowExactly<ArgumentNullException>(),
            _ => createViewLazy.Should().NotThrow()
                .Which.Operations.Should()
                .HaveCount(1)
                .And.Contain(op => op is CreateViewMigrationOperation, "Builder must have the corresponding operation")
        };
    }

    [Property(Arbitrary = [typeof(QueryGenerators)])]
    public void Delete(NonWhiteSpaceString providerName, DeleteQuery deleteQuery, string schema)
    {
        // Arrange
        Func<MigrationBuilder> deleteQueryLazy = () =>
        {
            MigrationBuilder builder = new(providerName.Item);
            return builder.Delete(deleteQuery, schema);
        };
        // Act
        object _ = (deleteQuery is null) switch
        {
            true => deleteQueryLazy.Should().ThrowExactly<ArgumentNullException>(),
            _ => deleteQueryLazy.Should()
                .NotThrow().Which
                .Operations.Should().HaveCount(1)
                .And.Contain(op => op is DeleteMigrationOperation, "Builder must have the corresponding operation")
        };
    }
}