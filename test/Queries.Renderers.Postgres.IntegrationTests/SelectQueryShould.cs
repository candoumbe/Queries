using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Npgsql;
using Queries.Core.Builders;
using Queries.Core.Parts.Columns;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Renderers.Postgres.IntegrationTests;

public class SelectQueryShould(PostgresDatabaseFixture fixture) : IAsyncLifetime, IClassFixture<PostgresDatabaseFixture>
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
            BatchQuery query = new BatchQuery([.. tableNames.Select(table => new NativeQuery($"DROP TABLE IF EXISTS {table};"))]);
            await using DbCommand command = new NpgsqlCommand(query.ForPostgres(), _connection);
            await command.ExecuteNonQueryAsync();
        }

        await _connection.CloseAsync();
    }

    public static TheoryData<string> EscapeNaughtyStringsCases => new(NaughtyStrings.TheNaughtyStrings.All);

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
}