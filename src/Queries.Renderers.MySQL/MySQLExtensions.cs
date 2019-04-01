
// ReSharper disable once CheckNamespace

using Queries.Core.Renderers;
using Queries.Renderers.MySQL;

namespace Queries.Core.Builders
{
    public static class MySQLExtensions
    {
        /// <summary>
        /// Computes the SQL string suitable for SQL SERVER
        /// </summary>
        /// <param name="query">the query to computes</param>
        /// <param name="settings">settings to use to render <see cref="query"/>.</param>
        /// <returns>a <see cref="string"/> representation of <see cref="query"/> to use to query a MySQL database engine</returns>
        public static string ForMySQL(this IQuery query, QueryRendererSettings settings) => new MySQLRenderer(settings).Render(query);
    }
}