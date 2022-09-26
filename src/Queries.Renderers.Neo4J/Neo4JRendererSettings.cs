using Queries.Core.Renderers;
using static Queries.Core.Renderers.PaginationKind;

namespace Queries.Renderers.Neo4J;

/// <summary>
/// Allows to customize the behavior of <see cref="Neo4JRenderer"/>.
/// </summary>
public class Neo4JRendererSettings : QueryRendererSettings
{
    /// <summary>
    /// Builds a new <see cref="Neo4JRendererSettings"/> instance.
    /// </summary>
    public Neo4JRendererSettings() : base(None)
    {
    }
}
