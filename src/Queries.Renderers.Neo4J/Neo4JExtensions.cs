using System;
using Queries.Core.Builders;
using Queries.Renderers.Neo4J;

namespace Queries.Core.Builders
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public static class Neo4JExtensions
    {
        public static string ForNeo4J(this IQuery query, bool prettyPrint) => new Neo4JRenderer(prettyPrint).Render(query);

    }
}

namespace Queries.Renderers.Neo4J
{
}
