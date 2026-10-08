using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using Npgsql;
using Queries.Core.Builders;
using Queries.Core.Parts.Columns;
using Xunit.Abstractions;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using static Queries.Core.Parts.Clauses.ClauseOperator;

namespace Queries.Renderers.Postgres.IntegrationTests;

public class SelectQueryShould(PostgresDatabaseFixture fixture, ITestOutputHelper outputHelper) : IAsyncLifetime, IClassFixture<PostgresDatabaseFixture>
{
    private NpgsqlConnection _connection;
    private const string TableName = "heroes";
    private readonly List<string> tableNames = [];

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        string connectionString = fixture.DatabaseContainer.GetConnectionString();
        _connection = new NpgsqlConnection(connectionString);
        await _connection.OpenAsync();
    }

    /// <inheritdoc />
    public async Task DisposeAsync()
    {
        if (tableNames.Count > 0)
        {
            string localConnectionString = fixture.DatabaseContainer.GetConnectionString();
            NpgsqlConnection localConnection = new (localConnectionString);
            await localConnection.OpenAsync();
            BatchQuery query = new ([.. tableNames.Select(table => new NativeQuery($"DROP TABLE IF EXISTS {table};"))]);
            await using DbCommand command = new NpgsqlCommand(query.ForPostgres(), localConnection);
            await command.ExecuteNonQueryAsync();
        }

        await _connection.CloseAsync();
    }

    public static TheoryData<string> EscapeNaughtyStringsCases => [.. NaughtyStrings.TheNaughtyStrings.All];

    [Theory]
    [MemberData(nameof(EscapeNaughtyStringsCases))]
    public async Task EscapeAllNaughtyStrings(string naughtyString)
    {
        // Arrange
        SelectQuery query = Select(naughtyString.Literal()).Build();
        string queryAsString = query.ForPostgres();

        DbCommand command = new NpgsqlCommand(queryAsString, _connection);
        DbDataReader reader = null;

        // Act
        Func<Task> runningQuery = async () => reader = await command.ExecuteReaderAsync();

        // Assert
        await runningQuery.Should().NotThrowAsync();
        reader.Should().NotBeNull();
        reader.HasRows.Should().BeTrue();
        ( await reader.ReadAsync() ).Should().BeTrue();
        reader[0].Should().Be(naughtyString);
    }

    [Fact]
    public async Task Be_parametrized_when_it_contains_problematic_values()
    {
        // Arrange
        DbCommand createTableCommand = new NpgsqlCommand(@"CREATE TABLE IF NOT EXISTS ""members"" (""id"" UUID PRIMARY KEY, ""name"" VARCHAR(255), ""powers"" TEXT);", _connection);
        await createTableCommand.ExecuteNonQueryAsync();
        tableNames.Add("members");

        SelectQuery query = Select("*")
                    .From("members")
                    .Where("invisibility".Literal(), EqualTo, "powers".Field())
                    .Build();

        string queryAsString = query.ForPostgres().Replace("SELECT", "PERFORM");
        outputHelper.WriteLine($"Executing query: '{queryAsString}'");

        DbCommand command = new NpgsqlCommand(queryAsString, _connection);
        DbDataReader reader = null;

        // Act
        Func<Task> runningQuery = async () => reader = await command.ExecuteReaderAsync();

        // Assert
        await runningQuery.Should().NotThrowAsync();
        reader.Should().NotBeNull();
    }
}