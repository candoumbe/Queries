using Queries.Core.Renderers;

namespace Queries.Renderers.SqlServer
{
    public class SqlServerRenderer : QueryRendererBase
    {
        public SqlServerRenderer(bool prettyPrint) : base(DatabaseType.SqlServer, prettyPrint)
        {}


        protected override string BeginEscapeWordString => "[";

        protected override string EndEscapeWordString => "]";

        protected override string ConcatOperator => "+";

        protected override string LengthFunctionName => "LEN";
    }
}