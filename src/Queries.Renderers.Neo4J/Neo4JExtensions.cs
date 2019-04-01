using Queries.Core.Renderers;
using Queries.Renderers.Neo4J;

namespace Queries.Core.Builders
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public static class Neo4JExtensions
    {
        public static string ForNeo4J(this IQuery query, QueryRendererSettings settings) => new Neo4JRenderer(settings).Render(query);
    }
}