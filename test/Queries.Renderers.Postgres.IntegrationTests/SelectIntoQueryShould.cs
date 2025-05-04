using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit.Categories;
using Xunit.Extensions.AssemblyFixture;
using Npgsql;
using Queries.Core.Builders;
using Queries.Core.Parts.Columns;
using Xunit.Abstractions;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Renderers.Postgres.IntegrationTests;

[IntegrationTest]
public class SelectIntoQueryShould(PostgresDatabaseFixture fixture, ITestOutputHelper outputHelper)
    : IAssemblyFixture<PostgresDatabaseFixture>, IAsyncLifetime
{
    private NpgsqlConnection _connection;
    private readonly string _tableName = $"heroes_{Guid.NewGuid():N}";
    private readonly List<string> tableNames = [];

    /// <inheritdoc />
    async Task IAsyncLifetime.InitializeAsync()
    {
        string connectionString = fixture.DatabaseContainer.GetConnectionString();

        _connection = new NpgsqlConnection(connectionString);
        await _connection.OpenAsync();
        // Drop the table if it already exists
        string dropTableQuery = $"DROP TABLE IF EXISTS {_tableName};";
        DbCommand command = new NpgsqlCommand(dropTableQuery, _connection);
        await command.ExecuteNonQueryAsync();
    }

    /// <inheritdoc />
    async Task IAsyncLifetime.DisposeAsync()
    {
        BatchQuery batch = new ([.. tableNames.Select(tableName => new NativeQuery($"DROP TABLE IF EXISTS {tableName};")) ]);
        DbCommand command = new NpgsqlCommand(batch.ForPostgres(), _connection);
        await command.ExecuteNonQueryAsync();
        await _connection.CloseAsync();

        tableNames.Clear();
    }

    [Fact]
    public async Task CreateCorrespondingRowsFromSelectQueryAsync()
    {
        // Arrange
        BatchQuery batch = new BatchQuery([
            new NativeQuery($"CREATE TABLE {_tableName} (id uuid PRIMARY KEY, firstname VARCHAR(50) NOT NULL, lastname VARCHAR(50) NOT NULL, alias VARCHAR(50) NOT NULL)"),
            InsertInto(_tableName).Values(
                "id".InsertValue(SelectColumn.UUID()),
                "firstname".InsertValue("Bruce".Literal()),
                "lastname".InsertValue("Wayne".Literal()),
                "alias".InsertValue("The dark knight".Literal())
                ),
            InsertInto(_tableName).Values(
                "id".InsertValue(SelectColumn.UUID()),
                "firstname".InsertValue("Barry".Literal()),
                "lastname".InsertValue("Allen".Literal()),
                "alias".InsertValue("The red scarlet".Literal())
            ),
            InsertInto(_tableName).Values(
                "id".InsertValue(SelectColumn.UUID()),
                "firstname".InsertValue("Clark".Literal()),
                "lastname".InsertValue("Kent".Literal()),
                "alias".InsertValue("The man of steel".Literal())
            ),
        ]);
        string batchAsString = batch.ForPostgres();
        NpgsqlCommand initDataCommand = new(batchAsString, _connection);
        outputHelper.WriteLine($"SQL query: '{batchAsString}'");
        await initDataCommand.ExecuteNonQueryAsync();

        string backupTableName = $"heroes_bck_{Guid.NewGuid():N}";
        SelectIntoQuery selectIntoQuery = SelectInto(backupTableName).From(_tableName.Table()).Build();
        tableNames.Add(backupTableName);
        string selectIntoQueryString = selectIntoQuery.ForPostgres();

        NpgsqlCommand selectIntoCommand = new(selectIntoQueryString, _connection);
        outputHelper.WriteLine($"SQL query: '{selectIntoQueryString}'");

        // Act
        Func<Task> runningQuery = async () => await selectIntoCommand.ExecuteNonQueryAsync();

        // Assert
        await runningQuery.Should().NotThrowAsync();

        SelectQuery select = Select("*").From(backupTableName).Build();
        DbCommand selectCommand = new NpgsqlCommand(select.ForPostgres(), _connection);
        await using DbDataReader reader = await selectCommand.ExecuteReaderAsync();
        using var _ = new AssertionScope();
        reader.HasRows.Should().BeTrue();
        (await reader.ReadAsync()).Should().BeTrue();
        reader["firstname"].Should().Be("Bruce");
        reader["lastname"].Should().Be("Wayne");
        reader["alias"].Should().Be("The dark knight");

        (await reader.ReadAsync()).Should().BeTrue();
        reader["firstname"].Should().Be("Barry");
        reader["lastname"].Should().Be("Allen");
        reader["alias"].Should().Be("The red scarlet");

        (await reader.ReadAsync()).Should().BeTrue();
        reader["firstname"].Should().Be("Clark");
        reader["lastname"].Should().Be("Kent");
        reader["alias"].Should().Be("The man of steel");

        (await reader.ReadAsync()).Should().BeFalse();
    }
}