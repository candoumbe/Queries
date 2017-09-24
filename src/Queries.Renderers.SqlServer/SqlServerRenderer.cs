using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Renderers;
using System;
using System.Collections;
using System.Collections.Generic;
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
        /// <param name="prettyPrint">defines how to render queries.</param>
        /// <remarks>
        ///     the <paramref name="prettyPrint"/> parameter defines how the queries will be rendered. When set to <c>true</c>,
        ///     each part of a staemtn will be layed in onto a newline.
        /// </remarks>
        public SqlServerRenderer(QueryRendererSettings settings) : base(settings)
        { }

        public SqlServerRenderer() : this (new QueryRendererSettings { DateFormatString = "yyyy-MM-dd", PrettyPrint = true })
        {

        }

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
            }
            StringBuilder sbParameters = new StringBuilder(visitor.Variables.Count() * 100);
            foreach (Variable variable in visitor.Variables)
            {
                sbParameters.Append($"DECLARE @{variable.Name} AS");
                switch (variable.Type)
                {
                    case VariableType.Numeric:
                        sbParameters.Append($" NUMERIC = {variable.Value};");
                        break;
                    case VariableType.String:
                        sbParameters.Append($" VARCHAR(8000) = '{variable.Value}';");
                        break;
                    case VariableType.Boolean:
                        break;
                    case VariableType.Date:
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


    }
}