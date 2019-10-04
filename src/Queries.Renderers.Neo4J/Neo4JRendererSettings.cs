using Queries.Core.Renderers;
using static Queries.Core.Renderers.PaginationKind;

namespace Queries.Renderers.Neo4J
{
    public class Neo4JRendererSettings : QueryRendererSettings
    {
        public Neo4JRendererSettings() : base(None)
        {
        }
    }
}
