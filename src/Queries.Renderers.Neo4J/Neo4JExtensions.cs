using Queries.Renderers.Neo4J;

namespace Queries.Core.Builders;

/// <summary>
/// Extension methods for <see cref="IQuery"/>
/// </summary>
public static class Neo4JExtensions
{
    /// <summary>
    /// Renders the Neo4J specific string for <paramref name="query"/> with the specific
    /// </summary>
    /// <param name="query">The query to render</param>
    /// <param name="settings">Settings to use in order to render the query</param>
    /// <returns>the Neo4J specific <see langword="string"/></returns>
    public static string ForNeo4J(this IQuery query, Neo4JRendererSettings settings) => new Neo4JRenderer(settings).Render(query);
}