using FluentAssertions;
using Queries.Core.Builders;
using Queries.Core.Parts;
using System;
using Xunit;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using Queries.Core.Extensions;

namespace Queries.Core.Tests.Parts
{
    [Collection("Unit tests")]
    public class SelectTableTests
    {

        [Fact]
        public void Ctor_Throws_ArgumentNullException_If_Argument_Is_Null()
        {
            // Arrange
            Action action = () => new SelectTable(null);

            // Assert
            action.ShouldThrow<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void SettingAlias()
        {
            // Arrange
            SelectQuery select = Select("firstname".Field(), "lastname".Field())
                                .From("people")
                                .Build();

            // Act
            SelectTable selectTable = new SelectTable(select)
                .As("p");

            // Assert
            selectTable.Alias.Should().Be("p");

        }

        [Fact]
        public void SettingSelect()
        {
            // Arrange
            SelectQuery select = Select("firstname".Field(), "lastname".Field())
                                .From("people")
                                .Build();

            // Act
            SelectTable selectTable = new SelectTable(select)
                .As("p");

            // Assert
            selectTable.Select.Should().BeSameAs(select);
        }
    }
}
