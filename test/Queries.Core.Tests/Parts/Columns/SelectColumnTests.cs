using System;
using Queries.Core.Parts.Columns;
using Xunit.Abstractions;
using Xunit.Categories;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using FsCheck.Xunit;
using FsCheck;
using FsCheck.Fluent;

namespace Queries.Core.Tests.Parts.Columns;

[UnitTest]
public class SelectColumnTests
{
    [Property]
    public Property Should_set_As_property(string newAlias)
    {
        // Arrange
        SelectColumn selectColumn = new(Select(1.Literal()));

        // Act
        selectColumn = selectColumn.As(newAlias);

        // Assert
        return (selectColumn.Alias == newAlias).ToProperty();
    }
}