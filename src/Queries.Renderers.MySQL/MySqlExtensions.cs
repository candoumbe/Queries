using Queries.Core.Parts.Clauses;
using Queries.Core.Renderers;
using Queries.Renderers.MySQL;

// ReSharper disable once CheckNamespace
namespace Queries.Core.Builders;

/// <summary>
/// Extensions methods related to 
/// </summary>
public static class MySqlExtensions
{
    /// <summary>
    /// Computes the SQL string suitable for MySql
    /// </summary>
    /// <param name="query">the query to compute</param>
    /// <param name="settings">settings to use to render <paramref name="query"/>.</param>
    /// <returns>a <see langword="string"/> representation of <paramref name="query"/> to use to query a MySQL database engine</returns>
    public static string ForMySql(this IQuery query, QueryRendererSettings settings) => new MySqlRenderer(settings).Render(query);

    /// <summary>
    /// Compiles the specified query
    /// </summary>
    /// <param name="query">The query to compile</param>
    /// <param name="settings"></param>
    /// <returns>a <see cref="CompiledQuery"/> which as the same semantic as <paramref name="query"/> but where all literals are replaced with <see cref="Variable"/>s</returns>
    public static CompiledQuery CompileForMySql(this IQuery query, MySqlRendererSettings settings) => new MySqlRenderer(settings).Compile(query);
}