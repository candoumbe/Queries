using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Queries.Builders;
using Queries.Parts;
using Queries.Parts.Columns;

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

        protected override string RenderColumnnameWithAlias(string columnName, string alias)
        {
            return String.Format("{0} {1}", columnName, alias);
        }


    }
}