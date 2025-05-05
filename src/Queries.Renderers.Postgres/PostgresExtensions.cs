using System;
using Queries.Core.Parts.Clauses;
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
    /// <param name="settings">Defines how to render <paramref name="query"/>.</param>
    /// <returns>A SQL string representation of the provided <paramref name="query"/> using Postgres syntax.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
    public static string ForPostgres(this IQuery query, PostgresRendererSettings settings = null) => new PostgresqlRenderer(settings).Render(query);

    /// <summary>
    /// Compiles the specified query
    /// </summary>
    /// <param name="query">The query to compile</param>
    /// <param name="settings"></param>
    /// <returns>a <see cref="CompiledQuery"/> which as the same semantic as <paramref name="query"/> but where all literals are replaced with <see cref="Variable"/>s</returns>
    /// <exception cref="ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
    public static CompiledQuery CompileForPostgres(this IQuery query, PostgresRendererSettings settings = null) => new PostgresqlRenderer(settings).Compile(query);
}