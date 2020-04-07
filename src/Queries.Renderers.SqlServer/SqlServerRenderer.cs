using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;
using Queries.Core.Renderers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static Queries.Core.Renderers.PaginationKind;

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
        public SqlServerRenderer(SqlServerRendererSettings settings = null)
            : base(settings ?? new SqlServerRendererSettings { DateFormatString = "yyyy-MM-dd", PrettyPrint = true } )
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
                case SelectQuery sq:
                    visitor.Visit(sq);
                    result = Render(sq);
                    break;
                case SelectQueryBase selectQueryBase:
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
                    throw new ArgumentOutOfRangeException(nameof(query), "Unknown type of query");
            }
            StringBuilder sbParameters = new StringBuilder(visitor.Variables.Count() * 100);

#if DEBUG
            if (visitor.Variables.Any())
            {
                Debug.Assert(visitor.Variables.All(x => x.Value != null), $"{nameof(visitor)}.{nameof(visitor.Variables)} must not contains variables with null value");
            }

#endif

            if (!Settings.SkipVariableDeclaration)
            {
                foreach (Variable variable in visitor.Variables)
                {
                    sbParameters.Append("DECLARE @").Append(variable.Name).Append(" AS");
                    switch (variable.Type)
                    {
                        case VariableType.Numeric:
                            sbParameters.Append(" NUMERIC = ").Append(variable.Value).Append(BatchStatementSeparator);
                            break;
                        case VariableType.String:
                            sbParameters.Append(" VARCHAR(8000) = '").Append(EscapeString(variable.Value.ToString())).Append("'")
                                .Append(BatchStatementSeparator);
                            break;
                        case VariableType.Boolean:
                            sbParameters.Append(" BIT = ")
                                .Append(true.Equals(variable.Value) ? "1" : "0")
                                .Append(BatchStatementSeparator);
                            break;
                        case VariableType.Date:
                            sbParameters.Append(" DATETIME = '").Append(EscapeString((variable.Value as DateTime?)?.ToString(Settings.DateFormatString)))
                                .Append(BatchStatementSeparator);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(variable), $"Unexpected {variable.Type} variable type");
                    }

                    if (Settings.PrettyPrint && sbParameters.Length > 0)
                    {
                        sbParameters.AppendLine();
                    }
                }
            }
            return sbParameters.Append(result).ToString();
        }

        public override CompiledQuery Compile(IQuery query)
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

            if (!Settings.SkipVariableDeclaration)
            {
                foreach (Variable variable in visitor.Variables)
                {
                    sbParameters.Append("DECLARE @").Append(variable.Name).Append(" AS");
                    switch (variable.Type)
                    {
                        case VariableType.Numeric:
                            sbParameters.Append(" NUMERIC = ").Append(variable.Value).Append(";");
                            break;
                        case VariableType.String:
                            sbParameters.Append(" VARCHAR(8000) = '").Append(EscapeString(variable.Value.ToString())).Append("';");
                            break;
                        case VariableType.Boolean:
                            break;
                        case VariableType.Date:
                            sbParameters.Append(" DATETIME = '").Append(EscapeString((variable.Value as DateTime?).Value.ToString(Settings.DateFormatString))).Append("'");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(variable), $"Unsupported variable type");
                    }

                    if (Settings.PrettyPrint && sbParameters.Length > 0)
                    {
                        sbParameters.AppendLine();
                    }
                }
            }
            return new CompiledQuery (result, visitor.Variables);
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