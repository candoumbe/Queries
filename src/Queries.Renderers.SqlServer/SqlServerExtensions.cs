using Queries.Core.Parts.Clauses;
using Queries.Renderers.SqlServer;

namespace Queries.Core;

/// <summary>
/// Extensions methods for <see cref="IQuery"/>
/// </summary>
public static class SqlServerExtensions
{
    /// <summary>
    /// Computes the SQL string suitable for SQL SERVER
    /// </summary>
    /// <param name="query">the query to computes</param>
    /// <param name="settings">Settings the renderer should use to generate the request </param>
    /// <returns></returns>
    public static string ForSqlServer(this IQuery query, SqlServerRendererSettings settings) => new SqlServerRenderer(settings).Render(query);

    /// <summary>
    /// Compiles the specified query
    /// </summary>
    /// <param name="query">The query to compile</param>
    /// <param name="settings"></param>
    /// <returns>a <see cref="CompiledQuery"/> which as the same semantic as <paramref name="query"/> but where all literals are replaced with <see cref="Variable"/>s</returns>
    public static CompiledQuery CompileForSqlServer(this IQuery query, SqlServerRendererSettings settings) => new SqlServerRenderer(settings).Compile(query);
}