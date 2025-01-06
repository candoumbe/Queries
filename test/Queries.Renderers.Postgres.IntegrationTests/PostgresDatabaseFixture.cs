using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;
using Testcontainers.PostgreSql;

namespace Queries.Renderers.Postgres.IntegrationTests;

/// <summary>
/// Fixture for Postgres database as a container
/// </summary>
public sealed class PostgresDatabaseFixture : IAsyncLifetime
{
	/// <summary>
	/// The underlying database container
	/// </summary>
	public PostgreSqlContainer DatabaseContainer { get; } = new PostgreSqlBuilder().WithImage("postgres:16").Build();

	/// <inheritdoc />
	public async Task InitializeAsync() => await DatabaseContainer.StartAsync().ConfigureAwait(false);

	/// <inheritdoc />
	async Task IAsyncLifetime.DisposeAsync() => await DatabaseContainer.StopAsync().ConfigureAwait(false);
}