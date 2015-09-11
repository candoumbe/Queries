using System;
using System.Linq;
using Queries.Builders;

namespace Queries.Renderers
{
    public class SqlServerRenderer : SqlRendererBase
    {

        public override string Render(SelectQueryBase query)
        {
            string result = Render(query, DatabaseType.SqlServer);
            return result;
        }

        protected override string GetConcatOperator()
        {
            return "+";
        }

        public override string EscapeName(string rawColumnName)
        {

            string escapedColumnName = String.Join(".",
                rawColumnName.Split(new[] {'.'}, StringSplitOptions.None)
                .Select(item => String.Format("[{0}]", item)));

            return escapedColumnName;
        }

    }
}