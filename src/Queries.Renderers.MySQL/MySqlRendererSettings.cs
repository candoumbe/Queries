using Queries.Core.Renderers;
using static Queries.Core.Renderers.PaginationKind;

namespace Queries.Renderers.MySQL
{
    public class MySqlRendererSettings : QueryRendererSettings
    {
        public MySqlRendererSettings(): base(Limit)
        {

        }
    }
}
