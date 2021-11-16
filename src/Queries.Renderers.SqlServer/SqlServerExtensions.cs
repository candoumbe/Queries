using Queries.Renderers.SqlServer;

namespace Queries.Core
{
    public static class SqlServerExtensions
    {
        /// <summary>
        /// Computes the SQL string suitable for SQL SERVER
        /// </summary>
        /// <param name="query">the query to computes</param>
        /// <param name="settings">Settings the renderer should use to genereate the request </param>
        /// <returns></returns>
        public static string ForSqlServer(this IQuery query, SqlServerRendererSettings settings) => new SqlServerRenderer(settings).Render(query);
    }
}