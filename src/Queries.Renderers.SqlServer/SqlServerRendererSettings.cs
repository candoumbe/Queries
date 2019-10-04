using Queries.Core.Renderers;
using static Queries.Core.Renderers.PaginationKind;

namespace Queries.Renderers.SqlServer
{
    public class SqlServerRendererSettings : QueryRendererSettings
    {
        public SqlServerRendererSettings() : base(Top)
        {

        }
    }
}
