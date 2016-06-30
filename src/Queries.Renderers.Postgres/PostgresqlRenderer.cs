using System;
using System.Text;
using Queries.Core.Renderers;
using Queries.Core.Parts.Functions;

namespace Queries.Renderers.Postgres
{
    public class PostgresqlRenderer : QueryRendererBase
    {
        /// <summary>
        /// Creates a new renderer for Postgres. <br/>
        /// 
        /// <para>
        ///     
        /// </para>
        /// </summary>
        /// <param name="prettyPrint">Defines how to render <see cref="IQuery"/></param>
        public PostgresqlRenderer(bool prettyPrint) : base(DatabaseType.Postgres, prettyPrint)
        {}


        protected override string EndEscapeWordString => @"""";

        protected override string ConcatOperator => "||";

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

        protected override string BeginEscapeWordString => @"""";


        protected override string RenderSubstringColumn(SubstringColumn substringColumn, bool renderAlias) => $"SUBSTRING({RenderColumn(substringColumn.Column, false)} FROM {substringColumn.Start}{(substringColumn.Length.HasValue ? $" FOR {substringColumn.Length.Value}" : string.Empty)})";
    }
     
}
