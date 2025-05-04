using System.Threading.Tasks;
using Bogus;
using DotNet.Testcontainers.Containers;
using Testcontainers.MsSql;

namespace Queries.Renderers.SqlServer.IntegrationTests;

/// <summary>
/// Fixture for MS SQL database as a container
/// </summary>
public sealed class MsSqlDatabaseFixture : IAsyncLifetime
{
	private readonly string _password;

	public MsSqlDatabaseFixture()
	{
		_password = new Faker().Internet.Password();
		DatabaseContainer = new MsSqlBuilder()
			.WithImage("mcr.microsoft.com/mssql/server:2022")
			.WithEnvironment("ACCEPT_EULA", "Y")
			.WithPassword(_password)
			.Build();
	}

	/// <summary>
	/// The underlying database container
	/// </summary>
	public MsSqlContainer DatabaseContainer { get; }

	/// <inheritdoc />
	public async Task InitializeAsync() => await DatabaseContainer.StartAsync().ConfigureAwait(false);

	/// <inheritdoc />
	async Task IAsyncLifetime.DisposeAsync() => await DatabaseContainer.StopAsync().ConfigureAwait(false);
}