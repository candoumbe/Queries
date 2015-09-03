using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using Queries.Builders;
using Queries.Parts;
using Queries.Parts.Columns;
using Queries.Parts.Joins;
using Queries.Validators;

namespace Queries.Renderers
{
    public abstract class SqlRendererBase : ISqlRenderer
    {
        public IValidate<SelectQuery> SelectQueryValidator{ get; private set; }

        
        /// <summary>
        /// <para>
        /// Escapes the given objectName using specific syntax.
        /// </para>
        /// </summary>
        /// <param name="objectName">Name of the object to escape</param>
        /// <returns>Escaped object name</returns>
        public virtual string EscapeName(string objectName)
        {
            string escapedColumnName = String.Join(".",
                objectName.Split(new[] { '.' }, StringSplitOptions.None).Select(item => String.Format("[{0}]", item)));

            return escapedColumnName;
        }

        public abstract string Render(SelectQueryBase query);

        protected virtual string Render(SelectQueryBase query, DatabaseType databaseType)
        {
            String queryString = String.Empty;

            if (query != null)
            {
                StringBuilder sb = new StringBuilder();



                string fieldsString = "*";
                if (query.Select !=  null && query.Select.Any())
                {
                    fieldsString = RenderColumns(query.Select);
                }

                if (query is SelectQuery)
                {
                    SelectQuery selectQuery = (SelectQuery) query;
                    if (selectQuery.From.Any())
                    {
                        string tableString = RenderTables(selectQuery.From);
                        int? limit = selectQuery.Limit;
                        
                        if (limit.HasValue)
                        {
                            switch (databaseType)
                            {
                                case DatabaseType.SqlServer:
                                case DatabaseType.SqlServerCompact:
                                    sb.AppendFormat("SELECT TOP {2} {0} FROM {1}", fieldsString, tableString, limit.Value);
                                    break;
                                case DatabaseType.Mysql:
                                case DatabaseType.MariaDb:
                                case DatabaseType.Postgres:
                                case DatabaseType.Sqlite:
                                case DatabaseType.Oracle:
                                    sb.AppendFormat("SELECT {0} FROM {1}", fieldsString, tableString);
                                    break;
                            }
                        }
                        else
                        {
                            sb.AppendFormat("SELECT {0} FROM {1}", fieldsString, tableString);
                        }
                    }
                    else
                    {
                        sb.AppendFormat("SELECT {0}", fieldsString);
                    }
                }
                else if (query is SelectIntoQuery)
                {
                    SelectIntoQuery selectInto = (SelectIntoQuery) query;

                    
                    sb.AppendFormat("SELECT {0} INTO {1} FROM {2}",
                        fieldsString,
                        RenderTablename(selectInto.Into, false),
                        RenderTables(selectInto.FromTable));
                }
            

                if (query.Joins.Any())
                {
                    string joinString = RenderJoins(query.Joins);
                    sb = sb.AppendFormat(" {0}", joinString);
                }
                

                if (query.Where != null)
                {
                    sb = sb.AppendFormat(" WHERE {0}", RenderWhere(query.Where));
                }

                if (query.Select != null)
                {
                    IEnumerable<AggregateColumn> aggregatedColumns = query.Select.OfType<AggregateColumn>();
                    IEnumerable<TableColumn> tableColumns = query.Select.OfType<TableColumn>();
                    if (aggregatedColumns.Any() && tableColumns.Any())
                    {
                        StringBuilder sbGroupBy = new StringBuilder();
                        IEnumerable<TableColumn> columnsToGroup = query.Select
                            .Where(col => (col is TableColumn))
                            .Cast<TableColumn>();

                        foreach (TableColumn column in columnsToGroup)
                        {
                            if (sbGroupBy.Length > 0)
                            {
                                sbGroupBy = sbGroupBy.Append(", ");
                            }
                            sbGroupBy = sbGroupBy.Append(EscapeName(column.Name));
                        }
                        sb = sb.AppendFormat(" GROUP BY {0}", sbGroupBy); 
                    }
                }

                if (query is SelectQuery)
                {
                    SelectQuery selectQuery = (SelectQuery) query;

                    if (selectQuery.Limit.HasValue)
                    {
                        switch (databaseType)
                        {
                            case DatabaseType.Postgres:
                            case DatabaseType.Mysql:
                            case DatabaseType.MariaDb:
                                sb.AppendFormat(" LIMIT {0}", selectQuery.Limit.Value);
                                break;
                        } 
                    }
                }
                
                if (query.Union != null)
                {
                    foreach (SelectQuery union in query.Union)
                    {
                        sb.Append(" UNION ").Append(Render(union));
                    }
                }

                queryString = sb.ToString();
                
            }            

            return queryString;
        }

        protected virtual string RenderJoins(IEnumerable<IJoin> joins)
        {
            joins = joins as IJoin[] ?? joins.ToArray();

            StringBuilder sbJoins = new StringBuilder();
            if (joins.Any())
            {
                foreach (var @join in joins)
                {
                    if (sbJoins.Length > 0)
                    {
                        sbJoins = sbJoins.Append(" ");
                    }

                    switch (@join.JoinType)
                    {
                        case JoinType.CrossJoin:
                            sbJoins = sbJoins.AppendFormat("CROSS JOIN {0}", RenderTablename(@join.Table, renderAlias: true));
                            break;
                        case JoinType.LeftOuterJoin:
                            sbJoins = sbJoins.AppendFormat("LEFT OUTER JOIN {0} ON {1}", RenderTablename(@join.Table, renderAlias: true),
                                RenderWhere(@join.On));
                            break;
                        case JoinType.RightOuterJoin:
                            sbJoins = sbJoins.AppendFormat("RIGHT OUTER JOIN {0} ON {1}", RenderTablename(@join.Table, renderAlias: true),
                                RenderWhere(@join.On));
                            break;
                        case JoinType.InnerJoin:
                            sbJoins = sbJoins.AppendFormat("INNER JOIN {0} ON {1}", RenderTablename(@join.Table, renderAlias: true),
                                RenderWhere(@join.On));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            return sbJoins.ToString();
        }

        protected virtual string RenderTables(IEnumerable<TableTerm> tables)
        {
            tables = tables as TableTerm[] ?? tables.ToArray();
            StringBuilder sbTables = new StringBuilder(tables.Count() * 25);
            foreach (TableTerm table in tables)
            {
                if (sbTables.Length != 0)
                {
                    sbTables = sbTables.Append(", ");
                }

                sbTables = String.IsNullOrWhiteSpace(table.Alias)
                    ? sbTables.Append(EscapeName(table.Name))
                    : sbTables.AppendFormat(RenderTablenameWithAlias(EscapeName(table.Name), EscapeName(table.Alias)));
            }
            return sbTables.ToString();
        }

        protected virtual string RenderColumns(IEnumerable<IColumn> columns)
        {
            columns = columns as IColumn[] ?? columns.ToArray();

            StringBuilder sbFields = new StringBuilder(columns.Count() * 25);

            foreach (IColumn column in columns)
            {
                if (sbFields.Length != 0)
                {
                    sbFields = sbFields.Append(", ");
                }
                sbFields.Append(RenderColumn(column, renderAlias: true));
            }

            return sbFields.ToString();
        }

        protected virtual string RenderWhereClause(WhereClause clause)
        {
            string whereString;

            switch (clause.Operator)
            {
                case WhereOperator.EqualTo:
                    whereString = String.Format("{0} = {1}", RenderColumn(clause.Column, false),
                        RenderColumn(clause.Constraint, false));
                    break;
                case WhereOperator.NotEqualTo:
                    whereString = String.Format("{0} <> {1}", RenderColumn(clause.Column, false),
                        RenderColumn(clause.Constraint, false));
                    break;
                case WhereOperator.LessThan:
                    whereString = String.Format("{0} < {1}", RenderColumn(clause.Column, false),
                        RenderColumn(clause.Constraint, false));
                    break;
                case WhereOperator.GreaterThan:
                    whereString = String.Format("{0} > {1}", RenderColumn(clause.Column, false),
                        RenderColumn(clause.Constraint, false));
                    break;
                case WhereOperator.Like:
                    whereString = String.Format("{0} LIKE {1}", RenderColumn(clause.Column, false),
                        RenderColumn(clause.Constraint, false));
                    break;
                case WhereOperator.NotLike:
                    whereString = String.Format("{0} NOT LIKE {1}", RenderColumn(clause.Column, false),
                        RenderColumn(clause.Constraint, false));
                    break;
                case WhereOperator.LessThanOrEqualTo:
                    whereString = String.Format("{0} <= {1}", RenderColumn(clause.Column, false),
                        RenderColumn(clause.Constraint, false));
                    break;
                case WhereOperator.GreaterThanOrEqualTo:
                    whereString = String.Format("{0} >= {1}", RenderColumn(clause.Column, false),
                        RenderColumn(clause.Constraint, false));
                    break;
                case WhereOperator.IsNull:
                    whereString = String.Format("{0} IS NULL", RenderColumn(clause.Column, false));
                    break;
                case WhereOperator.IsNotNull:
                    whereString = String.Format("{0} IS NOT NULL", RenderColumn(clause.Column, false));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return whereString;
        }
        
        protected virtual string RenderWhere(IClause clause)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (clause is WhereClause)
            {
                sbWhere.Append(RenderWhereClause((WhereClause)clause));
            }
            else if (clause is CompositeWhereClause)
            {
                CompositeWhereClause compositeClause = (CompositeWhereClause)clause;
                switch (compositeClause.Logic)
                {
                    case WhereLogic.And:
                        foreach (IClause innerClause in compositeClause.Clauses)
                        {
                            if (sbWhere.Length > 0)
                            {
                                sbWhere = sbWhere.Append(" AND ");
                            }
                            sbWhere = sbWhere.AppendFormat("{0}", RenderWhere(innerClause));
                        }

                        break;
                    case WhereLogic.Or:
                        foreach (IClause innerClause in compositeClause.Clauses)
                        {
                            if (sbWhere.Length > 0)
                            {
                                sbWhere = sbWhere.Append(" OR ");
                            }
                            sbWhere = sbWhere.AppendFormat("{0}", RenderWhere(innerClause));
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return sbWhere.Insert(0, '(').Insert(sbWhere.Length, ')').ToString();
        }

        protected virtual string RenderTablename(TableTerm table, bool renderAlias)
        {
            return !renderAlias || String.IsNullOrWhiteSpace(table.Alias)
                ? EscapeName(table.Name)
                : String.Format("{0} {1}", EscapeName(table.Name), EscapeName(table.Alias));
        }

        /// <summary>
        /// Renders the column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="renderAlias"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        protected virtual string RenderColumn(IColumn column, bool renderAlias)
        {
            string columnString = String.Empty;
            TableColumn tableColumn = column as TableColumn;
            if (tableColumn != null)
            {
                TableColumn tc = tableColumn;
                columnString = !renderAlias || String.IsNullOrWhiteSpace(tc.Alias)
                    ? EscapeName(tc.Name)
                    : RenderColumnnameWithAlias(EscapeName(tc.Name), EscapeName(tc.Alias));
            }
            else
            {
                LiteralColumn literalColumn = column as LiteralColumn;
                if (literalColumn != null)
                {
                    columnString = RenderLiteralColumn(literalColumn, renderAlias);
                }
                else
                {
                    AggregateColumn aggregateColumn = column as AggregateColumn;
                    if (aggregateColumn != null)
                    {
                        columnString = RenderAggregateColumn(aggregateColumn, renderAlias);
                    }
                    else
                    {
                        SelectColumn selectColumn = column as SelectColumn;
                        if(selectColumn != null)
                        {

                            columnString = RenderInlineSelect(selectColumn, renderAlias);
                            
                        }
                    }
                } 
            }


            return columnString;
        }
        
        protected virtual string RenderInlineSelect(SelectColumn inlineSelectQuery, bool renderAlias)
        {
            string columnString  = !renderAlias || String.IsNullOrWhiteSpace(inlineSelectQuery.Alias)
                        ? String.Format("({0})", Render(inlineSelectQuery.SelectQuery))
                        : RenderColumnnameWithAlias(String.Format("({0})", Render(inlineSelectQuery.SelectQuery)), EscapeName(inlineSelectQuery.Alias));
            


            return columnString;
        }

        protected virtual string RenderTablenameWithAlias(string tableName, string alias)
        {
            return String.Format("{0} {1}", tableName, alias);
        }

        protected virtual string RenderColumnnameWithAlias(string columnName, string alias)
        {
            return String.Format("{0} AS {1}", columnName, alias);
        }

        protected virtual string RenderAggregateColumn(AggregateColumn ac, bool renderAlias)
        {
            string columnString;
            switch (ac.Type)
            {
                case AggregateType.Min:
                    columnString = !renderAlias || String.IsNullOrWhiteSpace(ac.Column.Alias)
                        ? String.Format("MIN({0})", EscapeName(ac.Column.Name))
                        : RenderColumnnameWithAlias(String.Format("MIN({0})", EscapeName(ac.Column.Name)), EscapeName(ac.Column.Alias));
                    break;
                case AggregateType.Max:
                    columnString = !renderAlias || String.IsNullOrWhiteSpace(ac.Column.Alias)
                        ? String.Format("MAX({0})", EscapeName(ac.Column.Name))
                        : RenderColumnnameWithAlias(String.Format("MAX({0})", EscapeName(ac.Column.Name)), EscapeName(ac.Column.Alias));
                    break;
                case AggregateType.Average:
                    columnString = !renderAlias || String.IsNullOrWhiteSpace(ac.Column.Alias)
                        ? String.Format("AVG({0})", EscapeName(ac.Column.Name))
                        : RenderColumnnameWithAlias(String.Format("AVG({0})", EscapeName(ac.Column.Name)), EscapeName(ac.Column.Alias));
                    break;
                case AggregateType.Count:
                    columnString = !renderAlias || String.IsNullOrWhiteSpace(ac.Column.Alias)
                        ? String.Format("COUNT({0})", EscapeName(ac.Column.Name))
                        : RenderColumnnameWithAlias(String.Format("COUNT({0})", EscapeName(ac.Column.Name)), EscapeName(ac.Column.Alias));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return columnString;
        }

        protected virtual string RenderLiteralColumn(LiteralColumn literalColumn, bool renderAlias)
        {
            String columnString = String.Empty;
            LiteralColumn lc = literalColumn;
            object value = lc.Value;

            if (value is String)
            {
                if (renderAlias && !String.IsNullOrWhiteSpace(lc.Alias))
                {
                    columnString = String.Format("'{0}' AS {1}", EscapeString((string) value), EscapeName(lc.Alias));
                }
                else
                {
                    columnString = String.Format("'{0}'", EscapeString((String)value));
                }
            }
            else
            {
                if (renderAlias && !String.IsNullOrWhiteSpace(lc.Alias))
                {
                    columnString = String.Format("{0} AS {1}", value, EscapeName(lc.Alias));
                }
                else
                {
                    columnString = value.ToString();
                }
            }
            return columnString;
            
        }

        protected string EscapeString(string unescapedString)
        {
            return unescapedString.Replace("'", "''");
        }

        public virtual string Render(UpdateQuery updateQuery)
        {
            StringBuilder queryStringBuilder = new StringBuilder();

            if (updateQuery != null)
            {
                StringBuilder sbFieldsToUpdate = new StringBuilder(updateQuery.Set.Count * 50 * 2); // we assume column name will be around fifty characters
                foreach (UpdateFieldValue queryFieldValue in updateQuery.Set)
                {
                    if (sbFieldsToUpdate.Length > 0)
                    {
                        sbFieldsToUpdate = sbFieldsToUpdate.Append(", ");
                    }

                    sbFieldsToUpdate = sbFieldsToUpdate.AppendFormat("{0} = {1}", RenderColumn(queryFieldValue.Destination, false), RenderColumn(queryFieldValue.Source, false));
                }

                queryStringBuilder = queryStringBuilder.AppendFormat("UPDATE {0} SET {1}", RenderTablename(updateQuery.Table, renderAlias: false), sbFieldsToUpdate);

                if (updateQuery.Where != null)
                {
                    queryStringBuilder = queryStringBuilder.AppendFormat("WHERE {0}", RenderWhere(updateQuery.Where));
                }
                
            }


            return queryStringBuilder.ToString();
        }
    }
}