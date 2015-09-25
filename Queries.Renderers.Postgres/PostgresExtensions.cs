using Queries.Core;
using Queries.Renderers.Postgres;

// ReSharper disable once CheckNamespace
namespace Queries.Core.Builders
{
    public static class PostgresExtensions
    {
        /// <summary>
        /// Builds the SQL string suitable for <a href="www.postgres.com">Postgres</a> databases
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string ForPostgres(this IQuery query) => new PostgresqlRenderer().Render(query);

        

    }
}