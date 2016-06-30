using Queries.Renderers.Postgres;

// ReSharper disable once CheckNamespace
namespace Queries.Core.Builders
{
    public static class PostgresExtensions
    {
        /// <summary>
        /// Builds the SQL string suitable for <a href="www.postgres.com">Postgres</a> databases
        /// </summary>
        /// <param name="query">The query to render</param>
        /// <param name="prettyPrint"><code>true</code> to render a "prettier" SQL string</param>
        /// <returns></returns>
        public static string ForPostgres(this IQuery query, bool prettyPrint) => new PostgresqlRenderer(prettyPrint).Render(query);

        

    }
}