using System;
using System.Linq;
using Queries.Builders;

namespace Queries.Renderers
{
    public class PostgresqlRenderer : SqlRendererBase
    {
        public override string Render(SelectQueryBase query)
        {
            return Render(query, DatabaseType.Postgres);
        }

        public override string EscapeName(string rawColumnName)
        {
            string escapedColumnName = String.Join(".",
                rawColumnName.Split(new[] { '.' }, StringSplitOptions.None)
                .Select(token => String.Format("\"{0}\"", token)));

            return escapedColumnName;
        }

        protected override string GetConcatString()
        {
            return "||";
        }

        protected override string RenderColumnnameWithAlias(string columnName, string alias)
        {
            return String.Format("{0} {1}", columnName, alias);
        }




    }
}