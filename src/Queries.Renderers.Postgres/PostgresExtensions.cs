using Queries.Renderers.Postgres;

// ReSharper disable once CheckNamespace
namespace Queries.Core.Builders;

/// <summary>
/// Extension methods for Postgres
/// </summary>
public static class PostgresExtensions
{
    /// <summary>
    /// Builds the SQL string suitable for <see href="www.postgres.com">Postgres</see> databases
    /// </summary>
    /// <param name="query">The query to render</param>
    /// <param name="settings">Defines how to render <paramref name="query"/></param>
    /// <returns>A SQL string representation of the provided <paramref name="query"/> using Postgres syntax.</returns>
    public static string ForPostgres(this IQuery query, PostgresRendererSettings settings) => new PostgresqlRenderer(settings).Render(query);

    /// <summary>
    /// Renders the query into a SQL string suitable for Postgres databases using default <see cref="PostgresRendererSettings"/>.
    /// </summary>
    /// <param name="query">The query to render.</param>
    /// <returns>A SQL string representation of the provided <paramref name="query"/> using Postgres syntax.</returns>
    public static string ForPostgres(this IQuery query) => new PostgresqlRenderer(new PostgresRendererSettings()).Render(query);
}