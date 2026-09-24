using System;
using Queries.Core;
using Queries.Core.Exceptions;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;
using Queries.Core.Renderers;
using Queries.Renderers.Postgres.Builders;
using Queries.Renderers.Postgres.Parts.Columns;

using System.Linq;
using System.Text;
using Queries.Core.Builders;
using System.Collections.Generic;


#if DEBUG
using System.Diagnostics;
#endif

using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Renderers.Postgres;

/// <summary>
/// Renderer implementation that can output <see cref="IQuery"/> in a suitable format
/// usable with Postgresql database engine
/// </summary>
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
    public PostgresqlRenderer(PostgresRendererSettings settings = null)
        : base(settings ?? new PostgresRendererSettings { DateFormatString = "YYYY-mm-DD", PrettyPrint = true })
    { }

    ///<inheritdoc/>
    protected override string RenderColumn(IColumn column, bool renderAlias)
        => column is JsonFieldColumn json
                       ? RenderJsonColumn(json, renderAlias)
                       : base.RenderColumn(column, renderAlias);

    /// <summary>
    /// Renders a JSON column for a PostgreSQL query.
    /// </summary>
    /// <param name="json">The JSON field column to render.</param>
    /// <param name="renderAlias">Indicates whether to include the alias in the rendered output.</param>
    /// <returns>A string representing the rendered JSON column.</returns>
    protected virtual string RenderJsonColumn(JsonFieldColumn json, bool renderAlias)
    {
        string[] columnParts = json.Path.Split('.');
        string result;
        if (columnParts.Length == 1)
        {
            result = $"{RenderColumn(json.Column, renderAlias: false)} {(json.RenderAsString ? "->>" : "->")} '{columnParts.Single()}'";
        }
        else
        {
            string path = $"{string.Join(" -> ", columnParts.Take(columnParts.Length - 1).Select(EscapeName))} {(json.RenderAsString ? "->>" : "->")} '{columnParts.Last()}'";
            result = $"{RenderColumn(json.Column, renderAlias: false)} -> {path}";
        }

        return $"{result}{(renderAlias && !string.IsNullOrWhiteSpace(json.Alias) ? $" AS {EscapeName(json.Alias)}" : string.Empty)}";
    }

    ///<inheritdoc/>
    public override string Render(IQuery query)
    {
        if (query is ReturnQuery returnQuery)
        {
            return returnQuery.Return.Match(
                    columnBase =>
                    {
                        return columnBase switch
                        {
                            FieldColumn field => $"RETURN {RenderColumn(field, renderAlias: false)}",
                            Literal literal => $"RETURN {Render(Select(literal))["SELECT ".Length..]}",
                            null => "RETURN",
                            _ => throw new InvalidQueryException(),
                        };
                    },
                    select => $"RETURN {base.Render(select)}"
                );
        }
        else
        {
            string result = string.Empty;
            CollectVariableVisitor visitor = new();
            switch (query)
            {
                case SelectQuery sq:
                    if (Settings.Parametrization is not ParametrizationSettings.None)
                    {
                        visitor.Visit(sq);
                    }
                    result = Render(sq);
                    break;
                case SelectQueryBase selectQueryBase:
                    result = Render(selectQueryBase);
                    break;
                case CreateViewQuery createViewQuery:
                    if (Settings.Parametrization is not ParametrizationSettings.None)
                    {
                        visitor.Visit(createViewQuery.SelectQuery);
                    }
                    result = Render(createViewQuery);
                    break;
                case DeleteQuery deleteQuery:
                    if (Settings.Parametrization is not ParametrizationSettings.None)
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
                case NativeQuery nativeQuery:
                    result = nativeQuery.Statement;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(query), "Unknown type of query");
            }
            int variableCount = visitor.Variables.Count;
            StringBuilder sbParameters = new(variableCount * 100);

#if DEBUG
            if (variableCount > 0)
            {
                Debug.Assert(visitor.Variables.All(x => x.Value != null), $"{nameof(visitor)}.{nameof(visitor.Variables)} must not contains variables with null value");
            }

#endif
            if (Settings.Parametrization is ParametrizationSettings.Default && variableCount > 0)
            {

                sbParameters.AppendLine("DO $$")
                        .AppendLine("BEGIN")
                        .AppendLine("DECLARE");


                foreach (Variable variable in visitor.Variables)
                {
                    sbParameters.Append(variable.Name).Append(' ');
                    switch (variable.Type)
                    {
                        case VariableType.Numeric:
                            sbParameters = sbParameters.Append("NUMERIC := ").Append(variable.Value).AppendLine(BatchStatementSeparator);
                            break;
                        case VariableType.String:
                            sbParameters = sbParameters.Append("text := '").Append(EscapeString(variable.Value.ToString())).Append('\'')
                                .AppendLine(BatchStatementSeparator);
                            break;
                        case VariableType.Boolean:
                            sbParameters = sbParameters.Append("BIT := ")
                                .Append(true.Equals(variable.Value) ? '1' : '0')
                                .AppendLine(BatchStatementSeparator);
                            break;
                        case VariableType.Date:
                            sbParameters = sbParameters.Append("DATETIME := '").Append(EscapeString((variable.Value as DateTime?)?.ToString(Settings.DateFormatString)))
                                .AppendLine(BatchStatementSeparator);
                            break;
                        default:
                            throw new NotSupportedException($"Unexpected {variable.Type} variable type");
                    }


                }
                if (sbParameters.Length > 0)
                {
                    sbParameters.Append(result).AppendLine(BatchStatementSeparator);
                    sbParameters.AppendLine("END")
                                .Append("$$").Append(BatchStatementSeparator);
                }

                result = sbParameters.ToString();
            }

            return result;
        }
    }


    ///<inheritdoc/>
    protected override string RenderWhere(IWhereClause clause, int blockLevel = 0)
    {
        string result = string.Empty;

        switch (clause)
        {
            case WhereClause { Column: JsonFieldColumn json } where:
                result = where.Operator switch
                {
                    ClauseOperator.EqualTo => $"({RenderJsonColumn(new JsonFieldColumn(json.Column, json.Path, renderAsString: where.Constraint is StringColumn), renderAlias: false)} = {RenderColumn(where.Constraint, renderAlias: false)})",
                    _ => throw new NotSupportedException($"Unsupported '{where.Operator}' when rendering WHERE for '{nameof(JsonFieldColumn)}'"),
                };
                break;
            case WhereClause { Constraint: JsonFieldColumn jsonConstraint } where:
                switch (where.Operator)
                {
                    case ClauseOperator.EqualTo:
                        result = $"({RenderColumn(where.Column, renderAlias: false)} = {RenderJsonColumn(new JsonFieldColumn(jsonConstraint.Column, jsonConstraint.Path, renderAsString: where.Column is StringColumn), renderAlias: false)})";
                        break;
                    default:
                        break;
                }
                break;
            default:
                result = base.RenderWhere(clause);

                break;
        }

        return result;
    }

    ///<inheritdoc/>
    protected override string EndEscapeWordString => @"""";

    ///<inheritdoc/>
    protected override string ConcatOperator => "||";

    ///<inheritdoc/>
    protected override string RenderColumnNameWithAlias(string columnName, string alias) => $"{columnName} {alias}";

    ///<inheritdoc/>
    protected override string RenderUUIDValue() => "gen_random_uuid()";

    ///<inheritdoc/>
    protected override string RenderNullColumn(NullFunction nullColumn, bool renderAlias)
    {
        StringBuilder sbNullColumn = new();

        sbNullColumn = sbNullColumn.Append("COALESCE(")
            .Append(RenderColumn(nullColumn.Column, false)).Append(", ").Append(RenderColumn(nullColumn.DefaultValue, false))
            .Append(')');

        return renderAlias && !string.IsNullOrWhiteSpace(nullColumn.Alias)
            ? RenderColumnNameWithAlias(sbNullColumn.ToString(), EscapeName(nullColumn.Alias))
            : sbNullColumn.ToString();
    }

    ///<inheritdoc/>
    protected override string BeginEscapeWordString => @"""";

    ///<inheritdoc/>
    protected override string RenderVariable(Variable variable, bool renderAlias) => variable.Name;

    ///<inheritdoc/>
    protected override string RenderSubstringColumn(SubstringFunction substringColumn, bool renderAlias) => $"SUBSTRING({RenderColumn(substringColumn.Column, false)} FROM {substringColumn.Start}{(substringColumn.Length.HasValue ? $" FOR {substringColumn.Length.Value}" : string.Empty)})";

    ///<inheritdoc/>
    protected override string RenderPagination(int pageIndex, int pageSize)
    {
        StringBuilder sb = new StringBuilder()
            .Append("LIMIT ").Append(pageSize);

        if (pageIndex >= 1)
        {
            sb.Append(" OFFSET ").Append(pageSize);
        }

        if ((pageIndex - 1) >= 2)
        {
            sb.Append(" * ").Append(pageIndex - 1);
        }

        return sb.ToString();
    }
}