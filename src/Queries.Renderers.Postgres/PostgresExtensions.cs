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
    /// <returns></returns>
    public static string ForPostgres(this IQuery query, PostgresRendererSettings settings) => new PostgresqlRenderer(settings).Render(query);
}