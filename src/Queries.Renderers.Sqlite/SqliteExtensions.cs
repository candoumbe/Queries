using Queries.Renderers.Sqlite;

namespace Queries.Core.Builders;

/// <summary>
/// <see cref="IQuery"/> extension methods
/// </summary>
public static class SqliteExtensions
{
    /// <summary>
    /// Renders <paramref name="query"/> as a SQLite string after applying the specified <paramref name="settings"/>.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="settings"></param>
    /// <returns>a <see langword="string"/> suitable to use for querying a SQLite engine.</returns>
    public static string ForSqlite(this IQuery query, SqliteRendererSettings settings = null)
        => new SqliteRenderer(settings).Render(query);
}
