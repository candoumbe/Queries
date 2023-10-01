using Queries.Core.Builders;
using Queries.Core.Parts;
using Queries.Core.Parts.Columns;
using Queries.Core.Renderers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Queries.Renderers.Neo4J
{
    public class Neo4JRenderer : QueryRendererBase
    {
        public Neo4JRenderer(Neo4JRendererSettings settings) : base(settings)
        { }

        ///<inheritdoc/>
        protected override string BeginEscapeWordString => string.Empty;

        ///<inheritdoc/>
        protected override string EndEscapeWordString => string.Empty;

        ///<inheritdoc/>
        protected override string ConcatOperator => "+";

        ///<inheritdoc/>
        protected override string Render(SelectQueryBase query, int blockLevel = 0)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (query is not SelectQuery)
            {
                throw new NotSupportedException($"Only {nameof(SelectQuery)} queries are supported for Neo4J");
            }
            SelectQuery selectQuery = (SelectQuery)query;
            QueryWriter writer = new(prettyPrint: Settings.PrettyPrint);
            IEnumerable<IColumn> columns = query.Columns;
            IEnumerable<ITable> tables = selectQuery.Tables;

            NormalizeColumnAndTable(columns, selectQuery, tables);

            writer.WriteText("MATCH");
            writer.StartBlock();
            writer.WriteText(RenderTables(tables.ToArray()));
            writer.EndBlock();

            if (query.WhereCriteria is not null)
            {
                writer.WriteText("WHERE");
                writer.StartBlock();
                writer.WriteText(RenderWhere(query.WhereCriteria));
                writer.EndBlock();
            }

            writer.WriteText("RETURN");
            writer.StartBlock();
            writer.WriteText($"{RenderColumns(columns)}{BatchStatementSeparator}");
            writer.EndBlock();

            return writer.Value;

            static void NormalizeColumnAndTable(IEnumerable<IColumn> columns, SelectQuery selectQuery, IEnumerable<ITable> tables)
            {
                IEnumerable<IColumn> cols = columns as IColumn[] ?? columns.ToArray();
                if (cols.Count() == 1)
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
                                Literal lc = (Literal)column;
                                if ("*".Equals(lc.Value?.ToString()) && string.IsNullOrWhiteSpace(table.Alias))
                                {
                                    table.As(table.Name.Substring(0, 1).ToLower());
                                }
                            }
                        }
                    }
                }
            }
        }

        ///<inheritdoc/>
        protected override string RenderTablename(Table table, bool renderAlias)
            => $"({table.Alias}:{table.Name})";

        ///<inheritdoc/>
        protected override string RenderColumns(IEnumerable<IColumn> columns, int blockLevel = 0)
        {
            StringBuilder sbColumns = new();

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

            StringBuilder sbQuery = new();
            if (query.InsertedValue is IEnumerable<InsertedValue> values)
            {
                IDictionary<string, IColumn> map = values.ToDictionary(val => val.Column.Name, val => val.Value);
                StringBuilder sbCreate = new();
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

            StringBuilder sbQuery = new();
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
}