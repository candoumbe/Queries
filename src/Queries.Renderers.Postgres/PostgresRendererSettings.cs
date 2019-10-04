using Queries.Core.Renderers;
using static Queries.Core.Renderers.PaginationKind;

namespace Queries.Renderers.Postgres
{
    public class PostgresRendererSettings : QueryRendererSettings
    {
        public PostgresRendererSettings() : base(Limit)
        {

        }
    }
}
