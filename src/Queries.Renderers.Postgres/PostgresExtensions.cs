using Queries.Core.Exceptions;
using Queries.Core.Parts.Columns;
using Queries.Core.Renderers;
using Queries.Renderers.Postgres;
using Queries.Renderers.Postgres.Builders;
using System;
using static Queries.Core.Builders.Fluent.QueryBuilder;

// ReSharper disable once CheckNamespace
namespace Queries.Core.Builders
{
    public static class PostgresExtensions
    {
        /// <summary>
        /// Builds the SQL string suitable for <a href="www.postgres.com">Postgres</a> databases
        /// </summary>
        /// <param name="query">The query to render</param>
        /// <param name="settings">Defines how to render <paramref name="query"/></param>
        /// <returns></returns>
        public static string ForPostgres(this IQuery query, QueryRendererSettings settings) => new PostgresqlRenderer(settings).Render(query);
    }
}