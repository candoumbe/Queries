using Queries.Core.Renderers;
using static Queries.Core.Renderers.PaginationKind;

namespace Queries.Renderers.Sqlite
{
    public class SqliteRendererSettings : QueryRendererSettings
    {
        public SqliteRendererSettings() : base(Limit)
        {

        }
    }
}
