using Queries.Renderers.SqlServer;

// ReSharper disable once CheckNamespace
namespace Queries.Core.Builders
{
    public static class SqlServerExtensions
    {
        /// <summary>
        /// Computes the SQL string suitable for SQL SERVER
        /// </summary>
        /// <param name="query">the query to computes</param>
        /// <returns></returns>
        public static string ForSqlServer(this IQuery query) => new SqlServerRenderer().Render(query);
        
    }
}