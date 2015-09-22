using System;
using System.Linq;
using System.Text;
using Queries.Builders;
using Queries.Parts.Columns;

namespace Queries.Renderers
{
    public class PostgresqlRenderer : SqlRendererBase
    {

        public override string Render(SelectQueryBase query)
        {
            return Render(query, DatabaseType.Postgresql);
        }

        public override string EscapeName(string rawColumnName)
        {
            string escapedColumnName = String.Join(".",
                rawColumnName.Split(new[] { '.' }, StringSplitOptions.None)
                .Select(token => $@"""{token}"""));

            return escapedColumnName;
        }

        protected override string GetConcatOperator()
        {
            return "||";
        }

        protected override string RenderColumnnameWithAlias(string columnName, string alias)
        {
            return $"{columnName} {alias}";
        }


        protected override string RenderNullColumn(NullColumn nullColumn, bool renderAlias)
        {
            StringBuilder sbNullColumn = new StringBuilder();

            sbNullColumn = sbNullColumn.Append($"COALESCE({RenderColumn(nullColumn.Column, false)}, {RenderColumn(nullColumn.DefaultValue, false)})");

            string queryString = renderAlias && !String.IsNullOrWhiteSpace(nullColumn.Alias)
                ? RenderColumnnameWithAlias(sbNullColumn.ToString(), EscapeName(nullColumn.Alias))
                : sbNullColumn.ToString();

            return queryString;
        }



    }
}