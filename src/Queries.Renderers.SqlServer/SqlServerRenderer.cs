using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;
using Queries.Core.Renderers;

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Queries.Renderers.SqlServer;

/// <summary>
/// <see cref="IQueryRenderer"/> implementation for SQL Server
/// </summary>
public class SqlServerRenderer : QueryRendererBase
{
    /// <summary>
    /// Builds a new <see cref="SqlServerRenderer"/> instance.
    /// </summary>
    /// <param name="settings">defines how to render queries.</param>
    public SqlServerRenderer(SqlServerRendererSettings settings = null)
        : base(settings ?? new SqlServerRendererSettings { DateFormatString = "yyyy-MM-dd", PrettyPrint = true })
    { }

    ///<inheritdoc/>
    protected override string BeginEscapeWordString => "[";

    ///<inheritdoc/>
    protected override string EndEscapeWordString => "]";

    ///<inheritdoc/>
    protected override string ConcatOperator => "+";

    ///<inheritdoc/>
    protected override string LengthFunctionName => "LEN";

    ///<inheritdoc/>
    public override string Render(IQuery query)
    {
        string result = string.Empty;
        CollectVariableVisitor visitor = new();
        switch (query)
        {
            case SelectQuery sq:
                if (Settings.Parametrization != ParametrizationSettings.None)
                {
                    visitor.Visit(sq);
                }
                result = Render(sq);
                break;
            case SelectQueryBase selectQueryBase:
                result = Render(selectQueryBase);
                break;
            case CreateViewQuery createViewQuery:
                if (Settings.Parametrization != ParametrizationSettings.None)
                {
                    visitor.Visit(createViewQuery.SelectQuery);
                }
                result = Render(createViewQuery);
                break;
            case DeleteQuery deleteQuery:
                if (Settings.Parametrization != ParametrizationSettings.None)
                {
                    visitor.Visit(deleteQuery);
                }
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
        StringBuilder sbParameters = new(visitor.Variables.Count() * 100);

#if DEBUG
        if (visitor.Variables.Any())
        {
            Debug.Assert(visitor.Variables.All(x => x.Value != null), $"{nameof(visitor)}.{nameof(visitor.Variables)} must not contains variables with null value");
        }

#endif

        if (Settings.Parametrization == ParametrizationSettings.Default)
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
                        throw new ArgumentOutOfRangeException(nameof(variable), variable, $"Unexpected {variable.Type} variable type");
                }

                if (Settings.PrettyPrint && sbParameters.Length > 0)
                {
                    sbParameters.AppendLine();
                }
            }
        }
        return sbParameters.Append(result).ToString();
    }

    ///<inheritdoc/>
    public override CompiledQuery Compile(IQuery query)
    {
        string result = string.Empty;
        CollectVariableVisitor visitor = new();
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
                if (Settings.Parametrization != ParametrizationSettings.None)
                {
                    visitor.Visit(deleteQuery);
                }
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
        StringBuilder sbParameters = new(visitor.Variables.Count() * 100);

#if DEBUG
        if (visitor.Variables.Any())
        {
            Debug.Assert(visitor.Variables.All(x => x.Value != null), $"{nameof(visitor)}.{nameof(visitor.Variables)} must not contains variables with null value");
        }
#endif

        if (Settings.Parametrization == ParametrizationSettings.Default)
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
                        throw new ArgumentOutOfRangeException(nameof(variable), variable, $"Unsupported variable type");
                }

                if (Settings.PrettyPrint && sbParameters.Length > 0)
                {
                    sbParameters.AppendLine();
                }
            }
        }
        return new CompiledQuery(result, visitor.Variables);
    }

    ///<inheritdoc/>
    protected override string RenderVariable(Variable variable, bool renderAlias) => $"@{variable.Name}";

    ///<inheritdoc/>
    protected override string EscapeString(string unescapedString)
    {
        StringBuilder sbEscapedString = new(unescapedString);

        sbEscapedString = sbEscapedString
            .Replace("'", "''")
            .Replace("[", @"\[");

        return sbEscapedString.ToString();
    }
}