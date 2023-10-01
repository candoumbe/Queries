using Queries.Core.Renderers;

using static Queries.Core.Renderers.PaginationKind;


namespace Queries.Renderers.Sqlite;

/// <summary>
/// Allows to customize the behavior of <see cref="SqliteRenderer"/>.
/// </summary>
public class SqliteRendererSettings : QueryRendererSettings
{
    /// <summary>
    /// Builds a new <see cref="SqliteRendererSettings"/>
    /// </summary>
    public SqliteRendererSettings() : base(Limit)
    {
    }
}
