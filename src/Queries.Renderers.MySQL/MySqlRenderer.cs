using System;
using System.Linq;
using System.Text;
using Queries.Core.Renderers;
using Queries.Core.Parts.Functions;

namespace Queries.Renderers.MySQL
{
    /// <summary>
    /// Represents a renderer for MySQL queries, extending the base functionality
    /// to provide MySQL-specific SQL syntax rendering.
    /// </summary>
    /// <remarks>
    /// This class handles the rendering of SQL queries with MySQL-specific syntax,
    /// including the use of double quotes for escaping identifiers and the "||" 
    /// operator for string concatenation. It overrides methods to customize 
    /// the rendering of concatenated columns using the CONCAT function.
    /// </remarks>
    public class MySqlRenderer : QueryRendererBase
    {
        /// <summary>
        /// Builds a new <see cref="MySqlRenderer"/> instance.
        /// </summary>
        /// <param name="settings">Settings used to customize the behaviour of the renderer</param>
        public MySqlRenderer(QueryRendererSettings settings) : base(settings)
        {
        }

        /// <inheritdoc />
        protected override string BeginEscapeWordString => @"""";

        /// <inheritdoc />
        protected override string EndEscapeWordString => @"""";

        /// <inheritdoc />
        protected override string ConcatOperator => "||";

        /// <inheritdoc />
        protected override string RenderConcatColumn(ConcatFunction concatColumn, bool renderAlias)
        {
            if (concatColumn == null)
            {
                throw new ArgumentNullException(nameof(concatColumn));
            }

            StringBuilder sbConcat = new();
            sbConcat = concatColumn.Columns
                .Aggregate(sbConcat, (current, column) => current.Append($"{(current.Length > 0 ? ", " : string.Empty)}{RenderColumn(column, renderAlias: false)}"));

            sbConcat.Insert(0, "CONCAT(").Append(")");

            return renderAlias && !string.IsNullOrWhiteSpace(concatColumn.Alias)
                ? RenderColumnNameWithAlias(sbConcat.ToString(), EscapeName(concatColumn.Alias))
                : sbConcat.ToString();
        }
    }
}
