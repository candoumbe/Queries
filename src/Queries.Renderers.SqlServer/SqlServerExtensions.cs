using Queries.Core.Renderers;
using Queries.Renderers.SqlServer;

namespace Queries.Core
{
    public static class SqlServerExtensions
    {
        /// <summary>
        /// Computes the SQL string suitable for SQL SERVER
        /// </summary>
        /// <param name="query">the query to computes</param>
        /// <param name="prettyPrint"><code>true</code> to render a "prettier" SQL string</param>
        /// <returns></returns>
        public static string ForSqlServer(this IQuery query, QueryRendererSettings settings) => new SqlServerRenderer(settings).Render(query);
        
    }
}