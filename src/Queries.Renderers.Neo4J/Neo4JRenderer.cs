using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Queries.Core.Builders;
using Queries.Core.Parts;
using Queries.Core.Parts.Columns;
using Queries.Core.Renderers;

namespace Queries.Renderers.Neo4J;

/// <summary>
/// A renderer that can convert <see cref="Queries.Core.IQuery"/> to a Neo4J compatible <see langword="string"/>.
/// </summary>
public class Neo4JRenderer : QueryRendererBase
{
    /// <summary>
    /// Builds a new <see cref="Neo4JRenderer"/> instance.
    /// </summary>
    /// <param name="settings"></param>
    public Neo4JRenderer(Neo4JRendererSettings settings) : base(settings)
    { }

    ///<inheritdoc/>
    protected override string BeginEscapeWordString => string.Empty;

    ///<inheritdoc/>
    protected override string EndEscapeWordString => string.Empty;

    ///<inheritdoc/>
    protected override string ConcatOperator => "+";

    ///<inheritdoc/>
    protected override string Render(SelectQueryBase query)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        if (!(query is SelectQuery))
        {
            throw new NotSupportedException($"Only {nameof(SelectQuery)} queries are supported for Neo4J");
        }
        SelectQuery selectQuery = (SelectQuery) query;
        StringBuilder sbQuery = new StringBuilder();
        IEnumerable<IColumn> columns = query.Columns;
        IEnumerable<ITable> tables = selectQuery.Tables;

        NormalizeColumnAndTable(columns, selectQuery, tables);

        sbQuery.Append("MATCH ").Append(RenderTables(tables)).Append(" ").Append(Settings.PrettyPrint ? Environment.NewLine : string.Empty)
            .Append(query.WhereCriteria != null ? $"WHERE {RenderWhere(query.WhereCriteria)} {(Settings.PrettyPrint ? Environment.NewLine : string.Empty)}" : string.Empty)
            .Append("RETURN ").Append(RenderColumns(columns)).Append(BatchStatementSeparator);

        return sbQuery.ToString();
    }

    private static void NormalizeColumnAndTable(IEnumerable<IColumn> columns, SelectQuery selectQuery, IEnumerable<ITable> tables)
    {
        IEnumerable<IColumn> cols = columns as IColumn[] ?? columns.ToArray();
        if (cols.Once())
        {
            IColumn column = cols.Single();
            if (column is FieldColumn || column is Literal)
            {
                IEnumerable<ITable> tabs = tables as ITable[] ?? tables.ToArray();
                if (selectQuery.Tables.Count == 1 && tabs.Single() is Table table)
                {
                    if (column is FieldColumn fc)
                    {
                        if ("*".Equals(fc.Name))
                        {
                            if (string.IsNullOrWhiteSpace(table.Alias))
                            {
                                table.As(table.Name.Substring(0, 1).ToLower());
                            }
                            fc.Name = table.Alias;
                        }
                    }
                    else
                    {
                        Literal lc = (Literal) column;
                        if ("*".Equals(lc.Value?.ToString()) && string.IsNullOrWhiteSpace(table.Alias))
                        {
                            table.As(table.Name.Substring(0, 1).ToLower());
                        }
                    }
                }
            }
        }
    }

    ///<inheritdoc/>
    protected override string RenderTablename(Table table, bool renderAlias) => $"({table.Alias}:{table.Name})";

    ///<inheritdoc/>
    protected override string RenderColumns(IEnumerable<IColumn> columns)
    {
        StringBuilder sbColumns = new StringBuilder();

        sbColumns = columns.Aggregate(sbColumns,
            (current, column) => current
                .Append(current.Length > 0 ? ", " : string.Empty)
                .Append(RenderColumn(column, true)));

        return $"{sbColumns}";
    }

    ///<inheritdoc/>
    protected override string Render(InsertIntoQuery query)
    {
        if (query == null)
        {
            throw new ArgumentNullException(nameof(query));
        }
        //TODO validate the query

        StringBuilder sbQuery = new StringBuilder();
        if (query.InsertedValue is IEnumerable<InsertedValue> values)
        {
            IDictionary<string, IColumn> map = values
                .ToDictionary(val => val.Column.Name, val => val.Value);
            StringBuilder sbCreate = new StringBuilder();
            foreach (KeyValuePair<string, IColumn> kv in map)
            {
                IColumn columnValue = kv.Value;
                string valueString = RenderColumn(columnValue, false);
                sbCreate.Append(sbCreate.Length > 0 ? ", " : string.Empty).Append(kv.Key).Append(" : ").Append(valueString ?? "NULL");
            }

            sbQuery.Append($"CREATE ({query.TableName?.Substring(0, 1)?.ToLower()}:{query.TableName} {{{sbCreate}}})");
        }

        return sbQuery.ToString();
    }

    ///<inheritdoc/>
    protected override string Render(DeleteQuery deleteQuery)
    {
        if (deleteQuery == null)
        {
            throw new ArgumentNullException(nameof(deleteQuery));
        }

        StringBuilder sbQuery = new StringBuilder();
        string tableAlias = deleteQuery.Table?.Substring(0, 1)?.ToLower();
        sbQuery.Append($"MATCH {RenderTablenameWithAlias(deleteQuery.Table, tableAlias)} {(Settings.PrettyPrint ? Environment.NewLine : string.Empty)}");

        if (deleteQuery.Criteria != null)
        {
            sbQuery = sbQuery.Append($"WHERE {RenderWhere(deleteQuery.Criteria)} {(Settings.PrettyPrint ? Environment.NewLine : string.Empty)}");
        }
        sbQuery.Append("DELETE ").Append(tableAlias);

        return sbQuery.ToString();
    }

    ///<inheritdoc/>
    protected override string RenderTablenameWithAlias(string tableName, string alias)
        => $"({alias}:{tableName})";

    ///<inheritdoc/>
    protected override string RenderColumnnameWithAlias(string columnName, string alias)
        => $"{columnName}{(!string.IsNullOrWhiteSpace(alias) ? $" AS {alias}" : string.Empty)}";
}