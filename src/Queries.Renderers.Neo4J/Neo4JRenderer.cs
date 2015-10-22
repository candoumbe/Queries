using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Parts;
using Queries.Core.Parts.Columns;
using Queries.Core.Renderers;


namespace Queries.Renderers.Neo4J
{
    public class Neo4JRenderer : QueryRendererBase
    {
        public Neo4JRenderer(bool prettyPrint) : base(DatabaseType.Neo4J, prettyPrint)
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
            

            sbQuery.Append($"MATCH {RenderTables(tables)} {(PrettyPrint ? Environment.NewLine : string.Empty)}" +
                           $"RETURN {RenderColumns(columns)}{BatchStatementSeparator}");
            
            return sbQuery.ToString();
        }

        private static void NormalizeColumnAndTable(IEnumerable<IColumn> columns, SelectQuery selectQuery, IEnumerable<ITable> tables)
        {
            IEnumerable<IColumn> cols = columns as IColumn[] ?? columns.ToArray();
            if (cols.Count() == 1)
            {
                IColumn column = cols.Single();
                if (column is FieldColumn || column is LiteralColumn)
                {
                    IEnumerable<ITable> tabs = tables as ITable[] ?? tables.ToArray();
                    if (selectQuery.Tables.Count() == 1 && tabs.Single() is Table)
                    {
                        Table table = (Table) tabs.Single();

                        if (column is FieldColumn)
                        {
                            FieldColumn fc = (FieldColumn) column;
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
                            LiteralColumn lc = (LiteralColumn) column;
                            if ("*".Equals(lc.Value?.ToString()))
                            {
                                if (string.IsNullOrWhiteSpace(table.Alias))
                                {
                                    table.As(table.Name.Substring(0, 1).ToLower());
                                }
                                lc.Value = table.Alias;
                            }
                        }
                    }
                }
            }
        }

        protected override string RenderTables(IEnumerable<ITable> tables)
        {
            StringBuilder sbTables = new StringBuilder();

            foreach (ITable table in tables)
            {
                if (table is Table)
                {
                    sbTables.Append(RenderTablename((Table) table, true));
                }
                
            }

            return sbTables.ToString();
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


        protected override string RenderColumn(IColumn column, bool renderAlias)
        {
            string columnString = string.Empty;
            if (column is FieldColumn)
            {
                FieldColumn fc = (FieldColumn) column;
                columnString = RenderColumnnameWithAlias(fc.Name, fc.Alias);
            }

            return columnString;
        }


        protected override string Render(InsertIntoQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            //TODO validate the query

            StringBuilder sbQuery = new StringBuilder();
            if (query.InsertedValue is IEnumerable<InsertedValue>)
            {
                IEnumerable<InsertedValue> values = (IEnumerable<InsertedValue>) query.InsertedValue;

                IDictionary<string, LiteralColumn> map = values.ToDictionary(val => val.Column.Name, val => val.Value);
                StringBuilder sbCreate = new StringBuilder();
                foreach (KeyValuePair<string, LiteralColumn> kv in map)
                {
                    string valueString = (kv.Value.Value as DateTime?)?.ToString("O") ?? kv.Value.Value?.ToString();
                    sbCreate.Append($"{(sbCreate.Length > 0 ? ", " : string.Empty)}{kv.Key} : '{valueString}'");
                }

                sbQuery.Append($"CREATE ({query.TableName?.Substring(0, 1)?.ToLower()}:{query.TableName} {{{sbCreate}}})");
            }

            return sbQuery.ToString();
        }

        protected override string RenderColumnnameWithAlias(string columnName, string alias) 
            => $"{columnName}{(!string.IsNullOrWhiteSpace(alias) ? $" AS {alias}" : string.Empty)}";


    }
}