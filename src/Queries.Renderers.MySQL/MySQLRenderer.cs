using System;
using System.Linq;
using System.Text;
using Queries.Core.Renderers;
using Queries.Core.Parts.Functions;

namespace Queries.Renderers.MySQL
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class MySQLRenderer : QueryRendererBase
    {
        public MySQLRenderer(QueryRendererSettings settings) : base(settings)
        {
        }

        protected override string BeginEscapeWordString => @"""";
        protected override string EndEscapeWordString => @"""";
        protected override string ConcatOperator => "||";

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
}
