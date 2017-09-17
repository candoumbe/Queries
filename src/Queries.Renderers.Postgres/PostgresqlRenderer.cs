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
        /// <param name="settings">Defines how to render <see cref="IQuery"/></param>
        public PostgresqlRenderer(QueryRendererSettings settings) : base(settings)
        {}


        protected override string EndEscapeWordString => @"""";

        protected override string ConcatOperator => "||";

        protected override string RenderColumnnameWithAlias(string columnName, string alias) => $"{columnName} {alias}";
        
        protected override string RenderUUIDValue() => $"uuid_generate_v4()";
        
        protected override string RenderNullColumn(NullFunction nullColumn, bool renderAlias)
        {
            StringBuilder sbNullColumn = new StringBuilder();

            sbNullColumn = sbNullColumn.Append($"COALESCE({RenderColumn(nullColumn.Column, false)}, {RenderColumn(nullColumn.DefaultValue, false)})");

            string queryString = renderAlias && !String.IsNullOrWhiteSpace(nullColumn.Alias)
                ? RenderColumnnameWithAlias(sbNullColumn.ToString(), EscapeName(nullColumn.Alias))
                : sbNullColumn.ToString();

            return queryString;
        }

        protected override string BeginEscapeWordString => @"""";


        protected override string RenderSubstringColumn(SubstringFunction substringColumn, bool renderAlias) => $"SUBSTRING({RenderColumn(substringColumn.Column, false)} FROM {substringColumn.Start}{(substringColumn.Length.HasValue ? $" FOR {substringColumn.Length.Value}" : string.Empty)})";
    }
     
}
