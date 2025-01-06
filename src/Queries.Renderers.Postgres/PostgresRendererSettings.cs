using Queries.Core.Renderers;

using static Queries.Core.Renderers.PaginationKind;

namespace Queries.Renderers.Postgres;

/// <summary>
/// Extends <see cref="QueryRendererSettings"/> to customize the behavior of <see cref="PostgresqlRenderer"/>.
/// </summary>
public class PostgresRendererSettings : QueryRendererSettings
{
    /// <summary>
    /// Default size for VARCHAR columns.
    /// </summary>
#if NET
    public int DefaultVarcharLength { get; init; } = 1_000;
#else
    public int DefaultVarcharLength { get; set; } = 1_000;
#endif

    /// <summary>
    /// Builds a new <see cref="PostgresRendererSettings"/> instance.
    /// </summary>
    public PostgresRendererSettings() : base(Limit)
    {
    }
}
