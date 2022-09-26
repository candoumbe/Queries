using FluentAssertions;
using Queries.Core.Builders;
using Queries.Core.Parts;
using System;
using Xunit;
using Xunit.Categories;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Parts;

[UnitTest]
[Feature("Select table")]
public class SelectTableTests
{
    [Fact]
    public void Ctor_Throws_ArgumentNullException_If_Argument_Is_Null()
    {
        // Arrange
        Action action = () => new SelectTable(null);

        // Assert
        action.Should().Throw<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Feature("Alias")]
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

    [Fact]
    public void CloneTest()
    {
        // Arrange
        SelectQuery query = Select("firstname".Field(), "lastname".Field())
                            .From("people")
                            .Build();
        SelectTable source = new SelectTable(query)
                            .As("p");

        // Act
        ITable copy = source.Clone();

        // Assert
        SelectTable clone = copy.Should()
            .NotBeNull().And
            .BeAssignableTo<SelectTable>().Which;

        clone.Should()
            .BeEquivalentTo(source);
        clone.Select.Should()
            .NotBeSameAs(query, "select must be cloned as well").And
            .BeEquivalentTo(query);
    }
}
