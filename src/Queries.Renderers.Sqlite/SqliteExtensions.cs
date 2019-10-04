using Queries.Core.Renderers;
using Queries.Renderers.Sqlite;

namespace Queries.Core.Builders
{
    public static class SqliteExtensions
    {
        public static string ForSqlite(this IQuery query, SqliteRendererSettings settings = null)
            => new SqliteRenderer(settings).Render(query);
    }
}
