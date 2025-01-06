using System;
using System.Data.Common;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit.Categories;
using Xunit.Extensions.AssemblyFixture;
using Npgsql;
using Queries.Core.Builders;
using Xunit.Abstractions;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Renderers.Postgres.IntegrationTests;

[IntegrationTest]
public class InsertIntoQueryShould(PostgresDatabaseFixture fixture, ITestOutputHelper outputHelper)
    : IAssemblyFixture<PostgresDatabaseFixture>, IAsyncLifetime
{
    private NpgsqlConnection _connection;

    /// <inheritdoc />
    async Task IAsyncLifetime.InitializeAsync()
    {
        string connectionString = fixture.DatabaseContainer.GetConnectionString();

        _connection = new (connectionString);
        await _connection.OpenAsync();
        // Créer une table
        const string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS heroes (
                id SERIAL PRIMARY KEY,
                first_name VARCHAR(50) NOT NULL,
                last_name VARCHAR(50) NOT NULL,
                nickname VARCHAR(50) NOT NULL
            );";
        DbCommand command = new NpgsqlCommand(createTableQuery, _connection);
        await command.ExecuteNonQueryAsync();
    }

    /// <inheritdoc />
    async Task IAsyncLifetime.DisposeAsync()
    {
        string dropTableQuery = "DROP TABLE IF EXISTS heroes;";
        DbCommand command = new NpgsqlCommand(dropTableQuery, _connection);
        await command.ExecuteNonQueryAsync();
        await _connection.CloseAsync();
    }

    [Fact]
    public async Task CreateCorrespondingRowsFromSelectQueryAsync()
    {
        // Arrange
        InsertIntoQuery query = InsertInto("heroes")
            .Values(
                Select(1.Literal(), "Bruce".Literal(), "Wayne".Literal(), "Batman".Literal())
            )
            .Build();
        PostgresRendererSettings settings = new();
        string cmdText = query.ForPostgres(settings);

        outputHelper.WriteLine($"SQL query: '{cmdText}'");
        NpgsqlCommand command = new(cmdText, _connection);

        // Act
        int rowsAffected = 0;
        Func<Task> runningQuery = async () => rowsAffected = await command.ExecuteNonQueryAsync();

        // Assert
        await runningQuery.Should().NotThrowAsync();
        rowsAffected.Should().Be(1);

        
    }

    [Fact]
    public async Task CreateCorrespondingRowsFromInsertValuesAsync()
    {
        // Arrange
        InsertIntoQuery query = InsertInto("heroes")
            .Values(
                "id".InsertValue(1.Literal()),
                "first_name".InsertValue("Bruce".Literal()),
                "last_name".InsertValue("Wayne".Literal()),
                "nickname".InsertValue("Batman".Literal()))
            .Build();
        PostgresRendererSettings settings = new();
        string cmdText = query.ForPostgres(settings);

        outputHelper.WriteLine($"SQL query: '{cmdText}'");
        NpgsqlCommand command = new(cmdText, _connection);

        // Act
        int rowsAffected = 0;
        Func<Task> runningQuery = async () => rowsAffected = await command.ExecuteNonQueryAsync();

        // Assert
        await runningQuery.Should().NotThrowAsync();
        rowsAffected.Should().Be(1);
    }
}