using Queries.Core.Renderers;

namespace Queries.Renderers.SqlServer
{
    public class SqlServerRenderer : QueryRendererBase
    {
        public SqlServerRenderer() : base(DatabaseType.SqlServer)
        {}


        protected override string GetBeginEscapeWordString() => "[";

        protected override string GetEndingEscapeWordString() => "]";

        protected override string GetConcatOperator() => "+";

        
    }
}