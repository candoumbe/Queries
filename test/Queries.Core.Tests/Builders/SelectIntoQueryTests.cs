using FluentAssertions;

using Queries.Core.Builders;
using Queries.Core.Parts;

using System;

using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Builders
{
    [UnitTest]
    [Feature(nameof(SelectIntoQuery))]
    [Feature("Builder")]
    public class SelectIntoQueryTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public SelectIntoQueryTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void Given_table_is_null_Constructor_should_throw_ArgumentNullException()
        {
            // Act
            Action callingConstructorWhenTableIsNull = () => _ = new SelectIntoQuery((Table)null);

            // Assert
            callingConstructorWhenTableIsNull.Should()
                                             .ThrowExactly<ArgumentNullException>()
                                             .Which.ParamName.Should()
                                                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void Given_SelectIntoQuery_instance_When_calling_Then_Constructor_should_throw_ArgumentNullException()
        {
            // Act
            Action callingConstructorWhenTableIsNull = () => _ = new SelectIntoQuery((string)null);

            // Assert
            callingConstructorWhenTableIsNull.Should()
                                             .ThrowExactly<ArgumentNullException>()
                                             .Which.ParamName.Should()
                                                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void Given_SelectIntoQuery_When_calling_From_with_null_parameter_Then_ArgumentNullException_should_be_thrown()
        {
            // Arrange
            SelectIntoQuery selectIntoQuery = SelectInto("superherores");

            // Act
            Action action = () => selectIntoQuery.From(null);

            // Assert
            action.Should()
                  .ThrowExactly<ArgumentNullException>()
                  .Which.ParamName.Should()
                                  .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void Given_SelectIntoQuery_instance_When_calling_From_with_a_non_null_Table_parameter_Then_Source_property_should_be_set()
        {
            // Arrange
            Table table = "members".Table();
            SelectIntoQuery selectIntoQuery = SelectInto("superherores");

            // Act
            selectIntoQuery = selectIntoQuery.From(table)
                                             .Build();

            // Assert
            selectIntoQuery.Source.Should().Be(table);
        }
    }
}
