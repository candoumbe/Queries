using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;
using Queries.Core.Renderers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Queries.Renderers.SqlServer
{
    /// <summary>
    /// <see cref="IQueryRenderer"/> implementation for SQL Server
    /// </summary>
    public class SqlServerRenderer : QueryRendererBase
    {
        /// <summary>
        /// Builds a new <see cref="SqlServerRenderer"/> instance.
        /// </summary>
        /// <param name="settings">defines how to render queries.</param>
        /// <remarks>
        ///     the <paramref name="prettyPrint"/> parameter defines how the queries will be rendered. 
        ///     When set to <c>true</c>,
        ///     each part of a staemtn will be layed in onto a newline.
        /// </remarks>
        public SqlServerRenderer(QueryRendererSettings settings = null) 
            : base(settings ?? new QueryRendererSettings { DateFormatString = "yyyy-MM-dd", PrettyPrint = true })
        { }
        

        protected override string BeginEscapeWordString => "[";

        protected override string EndEscapeWordString => "]";

        protected override string ConcatOperator => "+";

        protected override string LengthFunctionName => "LEN";


        public override string Render(IQuery query)
        {
            string result = string.Empty;
            CollectVariableVisitor visitor = new CollectVariableVisitor();
            switch (query)
            {
                case SelectQueryBase selectQueryBase:
                    if (selectQueryBase is SelectQuery sq)
                    {
                        visitor.Visit(sq);
                        result = Render(sq);
                    }
                    result = Render(selectQueryBase);
                    break;
                case CreateViewQuery createViewQuery:
                    result = Render(createViewQuery);
                    break;
                case DeleteQuery deleteQuery:
                    visitor.Visit(deleteQuery);
                    result = Render(deleteQuery);
                    break;
                case UpdateQuery updateQuery:
                    result = Render(updateQuery);
                    break;
                case TruncateQuery truncateQuery:
                    result = Render(truncateQuery);
                    break;
                case InsertIntoQuery insertIntoQuery:
                    result = Render(insertIntoQuery);
                    break;
                case BatchQuery batchQuery:
                    result = Render(batchQuery);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown type of query");
            }
            StringBuilder sbParameters = new StringBuilder(visitor.Variables.Count() * 100);

#if DEBUG
            if (visitor.Variables.Any())
            {
                Debug.Assert(visitor.Variables.All(x => x.Value != null), $"{nameof(visitor)}.{nameof(visitor.Variables)} must not contains variables with null value");
            }

#endif

            foreach (Variable variable in visitor.Variables)
            {
                sbParameters.Append($"DECLARE @{variable.Name} AS");
                switch (variable.Type)
                {
                    case VariableType.Numeric:
                        sbParameters.Append($" NUMERIC = {variable.Value};");
                        break;
                    case VariableType.String:
                        sbParameters.Append($" VARCHAR(8000) = '{EscapeString(variable.Value.ToString())}';");
                        break;
                    case VariableType.Boolean:
                        break;
                    case VariableType.Date:
                        sbParameters.Append($" DATETIME = '{EscapeString((variable.Value as DateTime?).Value.ToString(Settings.DateFormatString))}'");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(variable), $"Unsupported variable type");
                        
                }

                if (Settings.PrettyPrint && sbParameters.Length > 0)
                {
                    sbParameters.AppendLine();
                }
            }
            return sbParameters.Append(result).ToString();
        }


        protected override string RenderVariable(Variable variable, bool renderAlias) => $"@{variable.Name}";

        protected override string EscapeString(string unescapedString)
        {
            StringBuilder sbEscapedString = new StringBuilder(unescapedString);

            sbEscapedString = sbEscapedString
                .Replace("'", "''")
                .Replace("[", @"\[");

            return sbEscapedString.ToString();
        }
    }
}