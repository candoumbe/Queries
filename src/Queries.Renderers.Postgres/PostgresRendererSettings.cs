using Queries.Core.Renderers;
using static Queries.Core.Renderers.PaginationKind;

namespace Queries.Renderers.Postgres;

/// <summary>
/// Extends <see cref="QueryRendererSettings"/> to customize the behavior of <see cref="PostgresqlRenderer"/>.
/// </summary>
public class PostgresRendererSettings : QueryRendererSettings
{
    /// <summary>
    /// Builds a new <see cref="PostgresRendererSettings"/> instance.
    /// </summary>
    public PostgresRendererSettings() : base(Limit)
    {
    }
}
