using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Queries.Core.Builders;
using Queries.Core.Parts;
using Queries.Core.Parts.Columns;
using Queries.Core.Renderers;


namespace Queries.Renderers.Neo4J
{
    public class Neo4JRenderer : QueryRendererBase
    {
        public Neo4JRenderer(QueryRendererSettings settings) : base(settings)
        { }

        protected override string BeginEscapeWordString => string.Empty;
        protected override string EndEscapeWordString => string.Empty;
        protected override string ConcatOperator => "+";


        protected override string Render(SelectQueryBase query)
        {
            if (query == null)
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
            

            sbQuery.Append($"MATCH {RenderTables(tables)} {(Settings.PrettyPrint ? Environment.NewLine : string.Empty)}" +
                           $"{(query.WhereCriteria != null ? $"WHERE {RenderWhere(query.WhereCriteria)} {(Settings.PrettyPrint ? Environment.NewLine : string.Empty)}" : string.Empty)}" +
                           $"RETURN {RenderColumns(columns)}{BatchStatementSeparator}");
            
            return sbQuery.ToString();
        }

        private static void NormalizeColumnAndTable(IEnumerable<IColumn> columns, SelectQuery selectQuery, IEnumerable<ITable> tables)
        {
            IEnumerable<IColumn> cols = columns as IColumn[] ?? columns.ToArray();
            if (cols.Count() == 1)
            {
                IColumn column = cols.Single();
                if (column is FieldColumn || column is Literal)
                {
                    IEnumerable<ITable> tabs = tables as ITable[] ?? tables.ToArray();
                    if (selectQuery.Tables.Count() == 1 && tabs.Single() is Table)
                    {
                        Table table = (Table) tabs.Single();

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
                            if ("*".Equals(lc.Value?.ToString()))
                            {
                                if (string.IsNullOrWhiteSpace(table.Alias))
                                {
                                    table.As(table.Name.Substring(0, 1).ToLower());
                                }
                                lc = new Literal(table.Alias).As(lc.Alias);
                            }
                        }
                    }
                }
            }
        }


        protected override string RenderTablename(Table table, bool renderAlias) 
            => $"({table.Alias}:{table.Name})";

        protected override string RenderColumns(IEnumerable<IColumn> columns)
        {
            StringBuilder sbColumns = new StringBuilder();

            sbColumns = columns.Aggregate(sbColumns,
                (current, column) => current.Append($"{(current.Length > 0 ? ", " : string.Empty)}{RenderColumn(column, true)}"));

            return $"{sbColumns}";
        }


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
                    sbCreate.Append($"{(sbCreate.Length > 0 ? ", " : string.Empty)}{kv.Key} : {(valueString ?? "NULL")}");
                }

                sbQuery.Append($"CREATE ({query.TableName?.Substring(0, 1)?.ToLower()}:{query.TableName} {{{sbCreate}}})");
            }

            return sbQuery.ToString();
        }


        protected override string Render(DeleteQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            StringBuilder sbQuery = new StringBuilder();
            string tableAlias = query.Table?.Substring(0, 1)?.ToLower();
            sbQuery.Append($"MATCH {RenderTablenameWithAlias(query.Table, tableAlias)} {(Settings.PrettyPrint ? Environment.NewLine : string.Empty)}");

            if (query.Criteria != null)
            {
                sbQuery = sbQuery.Append($"WHERE {RenderWhere(query.Criteria)} {(Settings.PrettyPrint ? Environment.NewLine : string.Empty)}");
            }
            sbQuery.Append($"DELETE {tableAlias}");

            return sbQuery.ToString();
        }


        protected override string RenderTablenameWithAlias(string tableName, string alias) => $"({alias}:{tableName})";

        protected override string RenderColumnnameWithAlias(string columnName, string alias) 
            => $"{columnName}{(!string.IsNullOrWhiteSpace(alias) ? $" AS {alias}" : string.Empty)}";


    }
}