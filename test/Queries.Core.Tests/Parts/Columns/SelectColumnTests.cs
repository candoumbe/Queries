using System;
using System.Reflection.Metadata;
using Queries.Core.Parts.Columns;
using Xunit.Abstractions;
using Xunit.Categories;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using FsCheck.Xunit;
using FsCheck;

namespace Queries.Core.Tests.Parts.Columns
{
    [UnitTest]
    public class SelectColumnTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public SelectColumnTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

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
}