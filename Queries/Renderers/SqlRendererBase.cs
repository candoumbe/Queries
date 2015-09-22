using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Queries.Builders;
using Queries.Exceptions;
using Queries.Extensions;
using Queries.Parts;
using Queries.Parts.Clauses;
using Queries.Parts.Columns;
using Queries.Parts.Joins;
using Queries.Parts.Sorting;
using Queries.Validators;

namespace Queries.Renderers
{
    public abstract class SqlRendererBase : ISqlRenderer
    {
        public IValidate<SelectQuery> SelectQueryValidator{ get; private set; }


        public bool PrettyPrint { get; set; }

        /// <summary>
        /// <para>
        /// Escapes the given objectName using specific syntax.
        /// </para>
        /// </summary>
        /// <param name="objectName">Name of the object to escape</param>
        /// <returns>Escaped object name</returns>
        public virtual string EscapeName(string objectName)
        {
            string escapedColumnName = string.Join(".",
                objectName.Split(new[] { '.' }, StringSplitOptions.None).Select(item => $"[{item}]"));

            return escapedColumnName;
        }

        public abstract string Render(SelectQueryBase query);

        protected virtual string Render(SelectQueryBase query, DatabaseType databaseType)
        {
            string queryString = string.Empty;

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
                                    sb.Append($"SELECT TOP {limit.Value} {fieldsString} ")
                                        .Append(PrettyPrint ? Environment.NewLine : String.Empty)
                                        .AppendFormat("FROM {0}", tableString);
                                    break;
                                case DatabaseType.Mysql:
                                case DatabaseType.MariaDb:
                                case DatabaseType.Postgresql:
                                case DatabaseType.Sqlite:
                                case DatabaseType.Oracle:
                                    sb.Append($"SELECT {fieldsString} ")
                                        .Append(PrettyPrint ? Environment.NewLine : String.Empty)
                                        .Append($"FROM {tableString}");
                                    break;
                            }
                        }
                        else
                        {
                            sb
                                .AppendFormat("SELECT {0} ", fieldsString)
                                .Append(PrettyPrint ? Environment.NewLine : String.Empty)
                                .AppendFormat("FROM {0}", tableString); 
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

                    sb.AppendFormat("SELECT {0} ", fieldsString)
                        .Append(PrettyPrint ? Environment.NewLine : String.Empty)
                        .AppendFormat("INTO {0} ", RenderTablename(selectInto.Into, false))
                        .Append(PrettyPrint ? Environment.NewLine : String.Empty)
                        .AppendFormat("FROM {0}", RenderTables(new[] { selectInto.From }));
                }
            

                if (query.Joins.Any())
                {
                    string joinString = RenderJoins(query.Joins);
                    sb = sb
                        .AppendFormat(" {0}", PrettyPrint ? Environment.NewLine : String.Empty)
                        .Append(joinString);
                }
                

                if (query.Where != null)
                {
                    sb = sb.Append($" {(PrettyPrint ? Environment.NewLine : "")}WHERE {RenderWhere(query.Where)}");
                }

                if (query.Select != null)
                {
                    IEnumerable<AggregateColumn> aggregatedColumns = query.Select.OfType<AggregateColumn>();
                    IEnumerable<FieldColumn> tableColumns = query.Select.OfType<FieldColumn>();
                    if (aggregatedColumns.Any() && tableColumns.Any())
                    {
                        StringBuilder sbGroupBy = new StringBuilder();
                        IEnumerable<FieldColumn> columnsToGroup = query.Select
                            .OfType<FieldColumn>();

                        foreach (FieldColumn column in columnsToGroup)
                        {
                            if (sbGroupBy.Length > 0)
                            {
                                sbGroupBy = sbGroupBy.Append(", ");
                            }
                            sbGroupBy = sbGroupBy.Append(EscapeName(column.Name));
                        }
                        sb = sb
                            .Append(' ')
                            .Append(PrettyPrint ? Environment.NewLine : String.Empty)
                            .AppendFormat("GROUP BY {0}", sbGroupBy); 
                    }
                }

                if (query.Having != null)
                {
                    sb
                        .Append(' ')
                        .Append(PrettyPrint ? Environment.NewLine : String.Empty)
                        .AppendFormat("HAVING {0}", RenderHaving(query.Having));
                }

                if (query.OrderBy.Any())
                {

                    StringBuilder sbOrderBy = new StringBuilder();

                    foreach (ISort sort in query.OrderBy)
                    {
                        if (sbOrderBy.Length > 0)
                        {
                            sbOrderBy = sbOrderBy.Append(", ");
                        }

                        sbOrderBy = sort.Direction == SortDirection.Descending 
                            ? sbOrderBy.AppendFormat("{0} DESC", RenderColumn(sort.Column, false))
                            : sbOrderBy.Append(RenderColumn(sort.Column, false));
                    }
                    sb.Append(" ")
                            .Append(PrettyPrint ? Environment.NewLine : String.Empty)
                            .AppendFormat("ORDER BY {0}", sbOrderBy);

                }


                if (query is SelectQuery)
                {
                    SelectQuery selectQuery = (SelectQuery) query;

                    if (selectQuery.Limit.HasValue &&
                        (databaseType == DatabaseType.Postgresql || databaseType == DatabaseType.Mysql ||
                         databaseType == DatabaseType.MariaDb))
                    {
                        sb
                            .Append(' ')
                            .Append(PrettyPrint ? Environment.NewLine : String.Empty)
                            .AppendFormat("LIMIT {0}", selectQuery.Limit.Value);
                    }
                }

                if (query.Union != null)
                {
                    foreach (SelectQuery union in query.Union)
                    {
                        sb
                            .Append(' ')
                            .Append(PrettyPrint ? Environment.NewLine : String.Empty)
                            .Append("UNION ")
                            .Append(PrettyPrint ? Environment.NewLine : String.Empty)
                            .Append(Render(union));
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

        protected virtual string RenderTables(IEnumerable<ITable> tables)
        {
            tables = tables as Table[] ?? tables.ToArray();
            StringBuilder sbTables = new StringBuilder(tables.Count() * 25);
            foreach (ITable table in tables)
            {

                if (sbTables.Length != 0)
                {
                    sbTables = sbTables.Append(", ");
                }

                if (table is Table)
                {
                    sbTables = String.IsNullOrWhiteSpace(table.Alias)
                        ? sbTables.Append(EscapeName(((Table)table).Name))
                        : sbTables.AppendFormat(RenderTablenameWithAlias(EscapeName(((Table)table).Name), EscapeName(table.Alias))); 
                } 
                else if (table is SelectTable)
                {
                    SelectTable selectTable = (SelectTable) table;
                    sbTables = String.IsNullOrWhiteSpace(table.Alias)
                        ? sbTables.Append(Render(selectTable.Select))
                        : sbTables.AppendFormat(RenderTablenameWithAlias(Render(selectTable.Select), EscapeName(selectTable.Alias))); 
                }
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

        protected virtual string RenderClause<T>(IClause<T> clause) where T : IColumn
        {
            string clauseString;

            switch (clause.Operator)
            {
                case ClauseOperator.EqualTo:
                    clauseString = $"{RenderColumn(clause.Column, false)} = {RenderColumn(clause.Constraint, false)}";
                    break;
                case ClauseOperator.NotEqualTo:
                    clauseString = $"{RenderColumn(clause.Column, false)} <> {RenderColumn(clause.Constraint, false)}";
                    break;
                case ClauseOperator.LessThan:
                    clauseString = $"{RenderColumn(clause.Column, false)} < {RenderColumn(clause.Constraint, false)}";
                    break;
                case ClauseOperator.GreaterThan:
                    clauseString = $"{RenderColumn(clause.Column, false)} > {RenderColumn(clause.Constraint, false)}";
                    break;
                case ClauseOperator.Like:
                    clauseString = $"{RenderColumn(clause.Column, false)} LIKE {RenderColumn(clause.Constraint, false)}";
                    break;
                case ClauseOperator.NotLike:
                    clauseString =
                        $"{RenderColumn(clause.Column, false)} NOT LIKE {RenderColumn(clause.Constraint, false)}";
                    break;
                case ClauseOperator.LessThanOrEqualTo:
                    clauseString = $"{RenderColumn(clause.Column, false)} <= {RenderColumn(clause.Constraint, false)}";
                    break;
                case ClauseOperator.GreaterThanOrEqualTo:
                    clauseString = $"{RenderColumn(clause.Column, false)} >= {RenderColumn(clause.Constraint, false)}";
                    break;
                case ClauseOperator.IsNull:
                    clauseString = $"{RenderColumn(clause.Column, false)} IS NULL";
                    break;
                case ClauseOperator.IsNotNull:
                    clauseString = $"{RenderColumn(clause.Column, false)} IS NOT NULL";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return clauseString;
        }
        
        protected virtual string RenderWhere(IWhereClause clause)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (clause is WhereClause)
            {
                sbWhere.Append(RenderClause((WhereClause)clause));
            }
            else if (clause is CompositeWhereClause)
            {
                CompositeWhereClause compositeClause = (CompositeWhereClause)clause;
                switch (compositeClause.Logic)
                {
                    case ClauseLogic.And:
                        foreach (IWhereClause innerClause in compositeClause.Clauses)
                        {
                            if (sbWhere.Length > 0)
                            {
                                sbWhere = sbWhere.Append(" AND ");
                            }
                            sbWhere = sbWhere.AppendFormat("{0}", RenderWhere(innerClause));
                        }

                        break;
                    case ClauseLogic.Or:
                        foreach (IWhereClause innerClause in compositeClause.Clauses)
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

        protected virtual string RenderHaving(IHavingClause clause)
        {
            StringBuilder sbHaving = new StringBuilder();

            if (clause is HavingClause)
            {
                sbHaving.Append(RenderClause((HavingClause)clause));
            }
            else if (clause is CompositeHavingClause)
            {
                CompositeHavingClause compositeClause = (CompositeHavingClause)clause;
                switch (compositeClause.Logic)
                {
                    case ClauseLogic.And:
                        foreach (IHavingClause innerClause in compositeClause.Clauses)
                        {
                            if (sbHaving.Length > 0)
                            {
                                sbHaving = sbHaving.Append(" AND ");
                            }
                            sbHaving = sbHaving.AppendFormat("{0}", RenderHaving(innerClause));
                        }

                        break;
                    case ClauseLogic.Or:
                        foreach (IHavingClause innerClause in compositeClause.Clauses)
                        {
                            if (sbHaving.Length > 0)
                            {
                                sbHaving = sbHaving.Append(" OR ");
                            }
                            sbHaving = sbHaving.AppendFormat("{0}", RenderHaving(innerClause));
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return sbHaving.Insert(0, '(').Insert(sbHaving.Length, ')').ToString();
        }

        


        protected virtual string RenderTablename(Table table, bool renderAlias)
        {
            return !renderAlias || String.IsNullOrWhiteSpace(table.Alias)
                ? EscapeName(table.Name)
                : $"{EscapeName(table.Name)} {EscapeName(table.Alias)}";
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
            if (column is FieldColumn)
            {
                FieldColumn tc = column as FieldColumn;
                columnString = !renderAlias || String.IsNullOrWhiteSpace(tc.Alias)
                    ? EscapeName(tc.Name)
                    : RenderColumnnameWithAlias(EscapeName(tc.Name), EscapeName(tc.Alias));
            }
            else if (column is LiteralColumn)
            {

                LiteralColumn literalColumn = column as LiteralColumn;
                columnString = RenderLiteralColumn(literalColumn, renderAlias);
            }
            else if (column is AggregateColumn)
            {
                AggregateColumn aggregateColumn = column as AggregateColumn;
                columnString = RenderAggregateColumn(aggregateColumn, renderAlias);
            }
            else if (column is SelectColumn)
            {
                SelectColumn selectColumn = column as SelectColumn;
                columnString = RenderInlineSelect(selectColumn, renderAlias);
            } else if (column is IFunctionColumn)
            {
                if (column is ConcatColumn)
                {
                    ConcatColumn concatColumn = column as ConcatColumn;
                    columnString = RenderConcatColumn(concatColumn, renderAlias);
                } else if (column is NullColumn)
                {
                    NullColumn nullColumn = column as NullColumn;
                    columnString = RenderNullColumn(nullColumn, renderAlias);
                } else if (column is LengthColumn)
                {
                    LengthColumn lengthColumn = column as LengthColumn;
                    columnString = RenderLengthColumn(lengthColumn, renderAlias);
                }
            } 


            return columnString;
        }

        protected string RenderLengthColumn(LengthColumn lengthColumn, bool renderAlias)
        {
            StringBuilder sbLengthColumn = new StringBuilder();

            sbLengthColumn = sbLengthColumn.AppendFormat("LENGTH({0})", RenderColumn(lengthColumn.Column, false));

            string queryString = renderAlias && !String.IsNullOrWhiteSpace(lengthColumn.Alias)
                ? RenderColumnnameWithAlias(sbLengthColumn.ToString(), EscapeName(lengthColumn.Alias))
                : sbLengthColumn.ToString();

            return queryString;
        }

        protected string RenderConcatColumn(ConcatColumn concatColumn, bool renderAlias)
        {
            StringBuilder sbConcat = new StringBuilder();
            foreach (IColumn column in concatColumn.Columns)
            {
                if (sbConcat.Length > 0)
                {
                    sbConcat = sbConcat.AppendFormat(" {0} ", GetConcatOperator());
                }
                sbConcat = sbConcat.Append(RenderColumn(column, false));
            }

            string queryString = renderAlias && !String.IsNullOrWhiteSpace(concatColumn.Alias)
                ? RenderColumnnameWithAlias(sbConcat.ToString(), EscapeName(concatColumn.Alias))
                : sbConcat.ToString();

            return queryString;

        }

        protected virtual string RenderNullColumn(NullColumn nullColumn, bool renderAlias)
        {
            StringBuilder sbNullColumn = new StringBuilder();

            sbNullColumn = sbNullColumn.AppendFormat("ISNULL({0}, {1})", RenderColumn(nullColumn.Column, false), RenderColumn(nullColumn.DefaultValue, false));
            
            string queryString = renderAlias && !String.IsNullOrWhiteSpace(nullColumn.Alias)
                ? RenderColumnnameWithAlias(sbNullColumn.ToString(), EscapeName(nullColumn.Alias))
                : sbNullColumn.ToString();

            return queryString;
        }

        protected abstract string GetConcatOperator();

        protected virtual string RenderInlineSelect(SelectColumn inlineSelectQuery, bool renderAlias)
        {
            string columnString  = !renderAlias || String.IsNullOrWhiteSpace(inlineSelectQuery.Alias)
                        ? $"({Render(inlineSelectQuery.SelectQuery)})"
                : RenderColumnnameWithAlias($"({Render(inlineSelectQuery.SelectQuery)})", EscapeName(inlineSelectQuery.Alias));
            


            return columnString;
        }

        protected virtual string RenderTablenameWithAlias(string tableName, string alias)
        {
            return $"{tableName} {alias}";
        }

        protected virtual string RenderColumnnameWithAlias(string columnName, string alias)
        {
            return $"{columnName} AS {alias}";
        }

        protected virtual string RenderAggregateColumn(AggregateColumn ac, bool renderAlias)
        {
            string columnString;
            switch (ac.Type)
            {
                case AggregateType.Min:
                    columnString = !renderAlias || String.IsNullOrWhiteSpace(ac.Alias)
                        ? $"MIN({EscapeName(ac.Column.Name)})"
                        : RenderColumnnameWithAlias($"MIN({EscapeName(ac.Column.Name)})", EscapeName(ac.Alias));
                    break;
                case AggregateType.Max:
                    columnString = !renderAlias || String.IsNullOrWhiteSpace(ac.Alias)
                        ? $"MAX({EscapeName(ac.Column.Name)})"
                        : RenderColumnnameWithAlias($"MAX({EscapeName(ac.Column.Name)})", EscapeName(ac.Alias));
                    break;
                case AggregateType.Average:
                    columnString = !renderAlias || String.IsNullOrWhiteSpace(ac.Alias)
                        ? $"AVG({EscapeName(ac.Column.Name)})"
                        : RenderColumnnameWithAlias($"AVG({EscapeName(ac.Column.Name)})", EscapeName(ac.Alias));
                    break;
                case AggregateType.Count:
                    columnString = !renderAlias || String.IsNullOrWhiteSpace(ac.Alias)
                        ? $"COUNT({EscapeName(ac.Column.Name)})"
                        : RenderColumnnameWithAlias($"COUNT({EscapeName(ac.Column.Name)})", EscapeName(ac.Alias));
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
                    columnString = $"'{EscapeString((string) value)}' AS {EscapeName(lc.Alias)}";
                }
                else
                {
                    columnString = $"'{EscapeString((String) value)}'";
                }
            }
            else
            {
                if (renderAlias && !String.IsNullOrWhiteSpace(lc.Alias))
                {
                    columnString = $"{value} AS {EscapeName(lc.Alias)}";
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
                        sbFieldsToUpdate = sbFieldsToUpdate
                            .Append(", ")
                            .Append(PrettyPrint ? Environment.NewLine : String.Empty);
                    }

                    sbFieldsToUpdate = sbFieldsToUpdate.AppendFormat("{0} = {1}", RenderColumn(queryFieldValue.Destination, false), RenderColumn(queryFieldValue.Source, false));
                }

                queryStringBuilder = queryStringBuilder
                    .AppendFormat("UPDATE {0} ", RenderTablename(updateQuery.Table, renderAlias: false))
                    .Append(PrettyPrint ? Environment.NewLine : String.Empty)
                    .AppendFormat("SET {0}", sbFieldsToUpdate);

                if (updateQuery.Where != null)
                {
                    queryStringBuilder = queryStringBuilder
                        .Append(" ")
                        .Append(PrettyPrint ? Environment.NewLine : String.Empty)
                        .AppendFormat("WHERE {0}", RenderWhere(updateQuery.Where));
                }
                
            }


            return queryStringBuilder.ToString();
        }


        public virtual string Render(CreateViewQuery query)
        {
            StringBuilder sb = new StringBuilder();

            if (query != null && new CreateViewQueryValidator().IsValid(query))
            {
                sb = sb.AppendFormat("CREATE VIEW {0} ", RenderTables(new ITable[] {query.Name.Table()}))
                    .Append(PrettyPrint ? Environment.NewLine : String.Empty)
                    .AppendFormat("AS ")
                    .Append(PrettyPrint ? Environment.NewLine : String.Empty)
                    .Append(Render(query.As));
            }

            return sb.ToString();
        }

        public string Render(TruncateQuery query)
        {
            string sbQuery = String.Empty;
            if (query != null && new TruncateQueryValidator().IsValid(query))
            {
                sbQuery = $"TRUNCATE TABLE {query.Name}";
            }

            return sbQuery;
        }

        public virtual string Render(DeleteQuery deleteQuery)
        {
            if (!new DeleteQueryValidator().IsValid(deleteQuery))
            {
                throw new InvalidQueryException("deleteQuery is not valid");
            }

            StringBuilder sbQuery = new StringBuilder();

            if (deleteQuery != null)
            {
                sbQuery = sbQuery.AppendFormat("DELETE FROM {0}", RenderTablename(deleteQuery.Table, false));

                if (deleteQuery.Where != null)
                {
                    sbQuery = sbQuery
                        .Append(" ")
                        .Append(PrettyPrint ? Environment.NewLine : String.Empty)
                        .AppendFormat("WHERE {0}", RenderWhere(deleteQuery.Where));
                } 
            }


            return sbQuery.ToString();

        }
    }
}