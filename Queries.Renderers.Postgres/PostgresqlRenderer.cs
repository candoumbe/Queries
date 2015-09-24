using System;
using System.Linq;
using System.Text;
using Queries.Core.Parts.Columns;
using Queries.Core.Renderers;

namespace Queries.Renderers.Postgres
{
    public class PostgresqlRenderer : QueryRendererBase
    {
        public PostgresqlRenderer() : base(DatabaseType.Postgres)
        {
            
        }


        protected override string GetEndingEscapeWordString() => @"""";

        protected override string GetConcatOperator() => "||";

        protected override string RenderColumnnameWithAlias(string columnName, string alias) => $"{columnName} {alias}";


        protected override string RenderNullColumn(NullColumn nullColumn, bool renderAlias)
        {
            StringBuilder sbNullColumn = new StringBuilder();

            sbNullColumn = sbNullColumn.Append($"COALESCE({RenderColumn(nullColumn.Column, false)}, {RenderColumn(nullColumn.DefaultValue, false)})");

            string queryString = renderAlias && !String.IsNullOrWhiteSpace(nullColumn.Alias)
                ? RenderColumnnameWithAlias(sbNullColumn.ToString(), EscapeName(nullColumn.Alias))
                : sbNullColumn.ToString();

            return queryString;
        }

        protected override string GetBeginEscapeWordString() => @"""";
    }
     
}
