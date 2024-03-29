﻿using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;
using Queries.Core.Renderers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Renderers.Sqlite;

/// <summary>
/// A renderer that can convert <see cref="IQuery"/> to a SQLite compatible <see langword="string"/>.
/// </summary>
/// <remarks>
/// By default, all dates are output in <c>yyyy-MM-dd</c>. This behavior can however be changed by passing a <see cref="SqliteRendererSettings"/> instance
/// to the constructor.
/// </remarks>
public class SqliteRenderer : QueryRendererBase
{
    /// <summary>
    /// Default name of the temporary table refererences by queries that use any <see cref="Variable"/>.
    /// </summary>
    private const string VariablesTempTablename = "_VARIABLES";

    /// <summary>
    /// Default name of the field in the table defined by <see cref="VariablesTempTablename"/> that will contains the name of all variables used by a query.
    /// </summary>
    private const string ParameterFieldName = "ParameterName";

    /// <summary>
    /// Builds a new <see cref="SqliteRenderer"/> instance that uses specified <paramref name="settings"/>.
    /// </summary>
    /// <param name="settings">Defines the settings to use when computing SQLite compatible <see langword="string"/>s</param>
    /// <remarks>
    /// If passing <see langword="null"/> as settings, some defaults values will be used :
    /// <list type="bullet">
    ///    <item><see cref="VariablesTempTablename"/> : Name of the table that will be used to store variables names and values if necessary</item>
    ///    <item><see cref="ParameterFieldName"/> : Name of field in the table <see cref="VariablesTempTablename"/> that will be used to store variables</item>
    /// </list>
    /// </remarks>
    public SqliteRenderer(SqliteRendererSettings settings = null)
        : base(settings ?? new SqliteRendererSettings { DateFormatString = "yyyy-MM-dd", PrettyPrint = true })
    { }

    ///<inheritdoc/>
    protected override string BeginEscapeWordString => @"""";

    ///<inheritdoc/>
    protected override string EndEscapeWordString => @"""";

    ///<inheritdoc/>
    protected override string ConcatOperator => "||";

    ///<inheritdoc/>
    public override string BatchStatementSeparator => ";";

    ///<inheritdoc/>
    protected override string RenderVariable(Variable variable, bool renderAlias) => $"@{variable.Name}";

    ///<inheritdoc/>
    public override string Render(IQuery query)
    {
        string result = string.Empty;
        ReplaceParameterBySelectQueryVisitor visitor = new(
            v => Select(Null("RealValue".Field(), "IntegerValue".Field(), "BlobValue".Field(), "TextValue".Field()))
                .From(VariablesTempTablename)
                .Where(ParameterFieldName.Field().EqualTo(v.Name))
                .Paginate(pageIndex: 1, pageSize: 1)
                .Build()
        );
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
                result = base.Render(query);
                break;
        }
        StringBuilder sbParameters = new(visitor.Variables.Count() * 100);
        if (visitor.Variables.Any())
        {
#if DEBUG
            Debug.Assert(visitor.Variables.All(x => x.Value != null), $"{nameof(visitor)}.{nameof(visitor.Variables)} must not contains variables with null value");
#endif

            IList<InsertIntoQuery> insertParameters = new List<InsertIntoQuery>(visitor.Variables.Count());
            IList<UpdateQuery> updateParameters = new List<UpdateQuery>(visitor.Variables.Count());

            foreach (Variable variable in visitor.Variables)
            {
                insertParameters.Add(
                    InsertInto(VariablesTempTablename).Values(ParameterFieldName.Field().InsertValue(variable.Name.Literal()))
                    .Build()
                );
                switch (variable.Type)
                {
                    case VariableType.Numeric:
                        updateParameters.Add(
                               Update(VariablesTempTablename).Set("RealValue".Field().UpdateValueTo(Convert.ToInt64(variable.Value)))
                               .Where(ParameterFieldName.Field().EqualTo(variable.Name))
                        );
                        break;
                    case VariableType.Date:
                    case VariableType.String:
                        updateParameters.Add(
                            Update(VariablesTempTablename).Set("TextValue".Field().UpdateValueTo(variable.Value.ToString()))
                                .Where(ParameterFieldName.Field().EqualTo(variable.Name))
                        );
                        break;
                    case VariableType.Boolean:
                        updateParameters.Add(
                            Update(VariablesTempTablename).Set("IntegerValue".Field().UpdateValueTo(Convert.ToInt32(variable.Value)))
                                .Where(ParameterFieldName.Field().EqualTo(variable.Name))
                        );
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Unsupported {variable.Type} as variable type");
                }
            }

            BatchQuery batch = new BatchQuery()
                .AddStatement("BEGIN".AsNative())
                .AddStatement("PRAGMA temp_store = 2".AsNative())
                .AddStatement($"CREATE TEMP TABLE {RenderTablename(VariablesTempTablename.Table(), renderAlias: false)}(ParameterName TEXT PRIMARY KEY, RealValue REAL, IntegerValue INTEGER, BlobValue BLOB, TextValue TEXT)"
                    .AsNative()
                )
                .AddStatements(insertParameters)
                .AddStatements(updateParameters)
                .AddStatement($"{result}".AsNative())
                .AddStatement($"DROP TABLE {RenderTablename(VariablesTempTablename.Table(), renderAlias: false)}".AsNative())
                .AddStatement("END".AsNative());

            result = Render(batch);
        }
        return result;
    }

    ///<inheritdoc/>
    protected override string RenderNullColumn(NullFunction nullColumn, bool renderAlias)
    {
        StringBuilder sbNullColumn = new();

        sbNullColumn = sbNullColumn.Append($"COALESCE({RenderColumn(nullColumn.Column, false)}, {RenderColumn(nullColumn.DefaultValue, false)}");
        foreach (IColumn defaultValue in nullColumn.AdditionalDefaultValues)
        {
            sbNullColumn
                .Append(", ")
                .Append(RenderColumn(defaultValue, false));
        }
        sbNullColumn.Append(")");

        return renderAlias && !string.IsNullOrWhiteSpace(nullColumn.Alias)
            ? RenderColumnnameWithAlias(sbNullColumn.ToString(), EscapeName(nullColumn.Alias))
            : sbNullColumn.ToString();
    }
}
