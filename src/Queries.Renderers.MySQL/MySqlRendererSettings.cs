using Queries.Core.Renderers;
using static Queries.Core.Renderers.PaginationKind;

namespace Queries.Renderers.MySQL;

/// <summary>
/// Can be used to customize the behavior of <see cref="MySQLRenderer"/> instances.
/// </summary>
public class MySqlRendererSettings : QueryRendererSettings
{
    /// <summary>
    /// Builds a new <see cref="MySqlRendererSettings"/> instance.
    /// </summary>
    public MySqlRendererSettings(): base(Limit)
    {
    }
}
