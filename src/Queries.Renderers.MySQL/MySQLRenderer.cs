using System;
using System.Linq;
using System.Text;
using Queries.Core.Renderers;
using Queries.Core.Parts.Functions;

namespace Queries.Renderers.MySQL;

/// <summary>
/// Renders <see cref="Core.IQuery"/> as a MySQL compatible <see langword="string"/>
/// </summary>
public class MySQLRenderer : QueryRendererBase
{
    /// <summary>
    /// Builds a new <see cref="MySQLRenderer"/> instance.
    /// </summary>
    /// <param name="settings">Settings that the renderer will use when rendering MySQL strings</param>
    public MySQLRenderer(QueryRendererSettings settings) : base(settings)
    {
    }

    ///<inheritdoc/>
    protected override string BeginEscapeWordString => @"""";

    ///<inheritdoc/>
    protected override string EndEscapeWordString => @"""";

    ///<inheritdoc/>
    protected override string ConcatOperator => "||";

    ///<inheritdoc/>
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
            ? RenderColumnnameWithAlias(sbConcat.ToString(), EscapeName(concatColumn.Alias))
            : sbConcat.ToString();
    }
}
