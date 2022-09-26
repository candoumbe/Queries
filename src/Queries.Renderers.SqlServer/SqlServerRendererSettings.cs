using Queries.Core.Renderers;
using static Queries.Core.Renderers.PaginationKind;

namespace Queries.Renderers.SqlServer;

/// <summary>
/// Wraps settings that can be used with <see cref="SqlServerRenderer"/>.
/// </summary>
public class SqlServerRendererSettings : QueryRendererSettings
{
    /// <summary>
    /// Builds a new <see cref="SqlServerRendererSettings"/> instance.
    /// </summary>
    public SqlServerRendererSettings() : base(Top)
    {

    }
}
