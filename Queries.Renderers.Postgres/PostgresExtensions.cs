using Queries.Renderers.Postgres;

// ReSharper disable once CheckNamespace
namespace Queries.Core.Builders
{
    public static class PostgresExtensions
    {
        public static string ForPostgres(this IQuery query) => new PostgresqlRenderer().Render(query);

        

    }
}