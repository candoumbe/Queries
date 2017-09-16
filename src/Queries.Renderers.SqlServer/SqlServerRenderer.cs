using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Renderers;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public SqlServerRenderer(bool prettyPrint) : base(DatabaseType.SqlServer, prettyPrint)
        { }


        protected override string BeginEscapeWordString => "[";

        protected override string EndEscapeWordString => "]";

        protected override string ConcatOperator => "+";

        protected override string LengthFunctionName => "LEN";

        protected override string Render(SelectQueryBase query)
        {
            StringBuilder sbResult = new StringBuilder();
            List<Variable> parameters = new List<Variable>();

            void GenerateCriterionFromWhereClause(IWhereClause whereClause, int currentParameterCount)
            {
                if (whereClause is WhereClause wc && wc.IsParameterized)
                {
                    Variable cp = null;
                    string parameterName = wc.Column is FieldColumn fc
                                ? !string.IsNullOrWhiteSpace(fc.Alias)
                                    ? fc.Alias
                                    : fc.Name
                                : $"p{currentParameterCount + 1}";

                    switch (wc.Constraint)
                    {
                        case StringColumn sc when sc.Value != null:
                            cp = new Variable(parameterName, VariableType.String, sc.Value);
                            break;
                        case NumericColumn nc when nc.Value != null:
                            cp = new Variable(parameterName, VariableType.Numeric, nc.Value);
                            break;
                        case DateTimeColumn dc when dc.Value != null:
                            cp = new Variable(parameterName, VariableType.Date, dc.Value);
                            break;
                        default:
                            break;
                    }

                    if (cp != null)
                    {

                        ((WhereClause)whereClause).Constraint = new LiteralColumn($"@{cp.Name}");
                        parameters.Add(cp);
                    }

                }
                else if (whereClause is CompositeWhereClause cwc)
                {
                    foreach (IWhereClause clause in cwc.Clauses)
                    {
                        GenerateCriterionFromWhereClause(clause, parameters.Count);
                    }
                }
            }

            if (query is SelectQuery sq)
            {
                if (sq.WhereCriteria != null)
                {
                    GenerateCriterionFromWhereClause(sq.WhereCriteria, parameters.Count);
                }

                foreach (Variable parameter in parameters)
                {
                    switch (parameter.Type)
                    {
                        case VariableType.Numeric:
                            sbResult.Append($"DECLARE @{parameter.Name} NUMERIC = {parameter.Value}");
                            break;
                        case VariableType.String:
                            sbResult.Append($"DECLARE @{parameter.Name} AS VARCHAR(8000) = '{parameter.Value.ToString().Replace("'", "''")}'");
                            break;
                        case VariableType.Boolean:
                            sbResult.Append($"DECLARE @{parameter.Name} BIT = {parameter.Value}");
                            break;
                        case VariableType.Date:
                            sbResult.Append($"DECLARE @{parameter.Name} DATETIME = '{parameter.Value}'");

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                if (sbResult.Length > 0)
                {
                    sbResult.Append(BatchStatementSeparator);
                }
                if (PrettyPrint && sbResult.Length > 0)
                {
                    sbResult.AppendLine();
                }

                sbResult.Append(base.Render(query));
            }
            else
            {
                sbResult = new StringBuilder(base.Render(query));
            }


            return sbResult.ToString();
        }

    }
}