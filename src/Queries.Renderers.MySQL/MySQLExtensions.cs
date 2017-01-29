
// ReSharper disable once CheckNamespace

using Queries.Renderers.MySQL;

namespace Queries.Core.Builders
{
    public static class MySQLExtensions
    {
        /// <summary>
        /// Computes the SQL string suitable for SQL SERVER
        /// </summary>
        /// <param name="query">the query to computes</param>
        /// <param name="prettyPrint"><code>true</code> to render a "prettier" SQL string</param>
        /// <returns></returns>
        public static string ForMySQL(this IQuery query, bool prettyPrint) => new MySQLRenderer(prettyPrint).Render(query);
        
    }
}