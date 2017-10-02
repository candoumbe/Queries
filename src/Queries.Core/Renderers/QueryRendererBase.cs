using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Queries.Core.Builders;
using Queries.Core.Builders.Fluent;
using Queries.Core.Exceptions;
using Queries.Core.Extensions;
using Queries.Core.Parts;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Joins;
using Queries.Core.Parts.Sorting;
using Queries.Core.Validators;
using Queries.Core.Parts.Functions;
using System.Reflection;
using Queries.Core.Attributes;

namespace Queries.Core.Renderers
{
    /// <summary>
    /// Base class for query renderers
    /// </summary>
    public abstract class QueryRendererBase : IQueryRenderer
    {

        public QueryRendererSettings Settings { get; }

        /// <summary>
        /// Builds a new <see cref="QueryRendererBase"/> instance.
        /// 
        /// </summary>
        /// <param name="databaseType">The <see cref="DatabaseType"/> to target</param>
        /// <param name="prettyPrint"><code>true</code> to render queries in a "pretty" fashion</param>
        protected QueryRendererBase(QueryRendererSettings settings) => Settings = settings;


        /// <summary>
        /// <para>
        /// Escapes the given objectName using specific syntax.
        /// </para>
        /// </summary>
        /// <param name="rawName">Name of the object to escape</param>
        /// <returns>Escaped object name</returns>
        protected virtual string EscapeName(string rawName)
        {

            string escapedColumnName = string.Empty;

            if (rawName != null)
            {
                string[] rawNameParts = rawName.Split(new[] { '.' }, StringSplitOptions.None);
                StringBuilder sb = new StringBuilder();

                foreach (string namePart in rawNameParts)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(".");
                    }

                    sb.Append("*".Equals(namePart.Trim())
                        ? "*"
                        : $"{BeginEscapeWordString}{namePart}{EndEscapeWordString}");
                }

                escapedColumnName = sb.ToString();
            }

            return escapedColumnName;
        }

        public virtual string Render(IQuery query)
        {
            string result = string.Empty;
            switch (query)
            {
                case SelectQueryBase selectQueryBase:
                    result = Render(selectQueryBase);
                    break;
                case CreateViewQuery createViewQuery:
                    result = Render(createViewQuery);
                    break;
                case DeleteQuery deleteQuery:
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
            }

            return result;
        }

        protected virtual string Render(InsertIntoQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), $"{nameof(query)} cannot be null");
            }

            string queryString = string.Empty;

            if (query.InsertedValue is SelectQuery selectQuery)
            {
                queryString = $"INSERT INTO {RenderTablename(query.TableName.Table(), renderAlias: false)} {(Settings.PrettyPrint ? Environment.NewLine : string.Empty)}" +
                    $"{Render(selectQuery)}";
            }
            else if (query.InsertedValue is IEnumerable<InsertedValue> values)
            {
                StringBuilder sbColumns = new StringBuilder();
                StringBuilder sbValues = new StringBuilder();
                foreach (InsertedValue insertedValue in values)
                {
                    sbValues.Append($"{(sbValues.Length > 0 ? ", " : string.Empty)}{RenderColumn(insertedValue.Value, renderAlias: false)}");
                    sbColumns.Append($"{(sbColumns.Length > 0 ? ", " : string.Empty)}{RenderColumn(insertedValue.Column, renderAlias: false)}");

                    queryString = $"INSERT INTO {RenderTablename(query.TableName.Table(), renderAlias: false)} ({sbColumns}) {(Settings.PrettyPrint ? Environment.NewLine : string.Empty)}" +
                        $"VALUES ({sbValues})";
                }
            }

            return queryString;
        }




        protected virtual string Render(SelectQueryBase query)
        {
            string queryString = string.Empty;
            
            if (query != null)
            {
                StringBuilder sb = new StringBuilder();

                string fieldsString = "*";
                if (query.Columns != null && query.Columns.Any())
                {
                    fieldsString = RenderColumns(query.Columns);
                }

                if (query is SelectQuery)
                {
                    SelectQuery selectQuery = (SelectQuery)query;
                    if (selectQuery.Tables.Any())
                    {
                        string tableString = RenderTables(selectQuery.Tables);
                        int? limit = selectQuery.NbRows;

                        if (limit.HasValue)
                        {

                            sb.Append($"SELECT TOP {limit.Value} {fieldsString} ")
                                .Append(Settings.PrettyPrint ? Environment.NewLine : string.Empty)
                                .AppendFormat("FROM {0}", tableString);

                        }
                        else
                        {
                            sb
                                .AppendFormat("SELECT {0} ", fieldsString)
                                .Append(Settings.PrettyPrint ? Environment.NewLine : string.Empty)
                                .AppendFormat("FROM {0}", tableString);
                        }
                    }
                    else
                    {
                        sb.AppendFormat("SELECT {0}", fieldsString);
                    }
                }
                else if (query is SelectIntoQuery selectInto)
                {
                    sb.Append(
                        $"SELECT {fieldsString} {(Settings.PrettyPrint ? Environment.NewLine : string.Empty)}" +
                        $"INTO {RenderTablename(selectInto.Destination, false)} {(Settings.PrettyPrint ? Environment.NewLine : string.Empty)}" +
                        $"FROM {RenderTables(new[] { selectInto.Source })}");

                }


                if (query.Joins.Any())
                {
                    string joinString = RenderJoins(query.Joins);
                    sb = sb.Append($" {(Settings.PrettyPrint ? Environment.NewLine : string.Empty)}{joinString}");
                }


                if (query.WhereCriteria != null)
                {
                    sb = sb.Append($" {(Settings.PrettyPrint ? Environment.NewLine : "")}WHERE {RenderWhere(query.WhereCriteria)}");
                }

                if (query.Columns != null)
                {
                    IEnumerable<AggregateFunction> aggregatedColumns = query.Columns.OfType<AggregateFunction>();
                    IEnumerable<FieldColumn> tableColumns = query.Columns.OfType<FieldColumn>();
                    if (aggregatedColumns.Any() && tableColumns.Any())
                    {
                        StringBuilder sbGroupBy = new StringBuilder();
                        IEnumerable<FieldColumn> columnsToGroup = query.Columns
                            .OfType<FieldColumn>();

                        foreach (FieldColumn column in columnsToGroup)
                        {
                            if (sbGroupBy.Length > 0)
                            {
                                sbGroupBy = sbGroupBy.Append(", ");
                            }
                            sbGroupBy = sbGroupBy.Append(EscapeName(column.Name));
                        }
                        sb = sb.Append($" {(Settings.PrettyPrint ? Environment.NewLine : string.Empty)}" +
                            $"GROUP BY {sbGroupBy}");
                    }
                }

                if (query.HavingCriteria != null)
                {
                    sb.Append(
                        $" {(Settings.PrettyPrint ? Environment.NewLine : string.Empty)}HAVING {RenderHaving(query.HavingCriteria)}");

                }

                if (query.Sorts.Any())
                {

                    StringBuilder sbOrderBy = new StringBuilder();

                    foreach (ISort sort in query.Sorts)
                    {
                        if (sbOrderBy.Length > 0)
                        {
                            sbOrderBy = sbOrderBy.Append(", ");
                        }

                        sbOrderBy = sort.Direction == SortDirection.Descending
                            ? sbOrderBy.Append($"{RenderColumn(sort.Column, false)} DESC")
                            : sbOrderBy.Append(RenderColumn(sort.Column, false));
                    }
                    sb.Append(" ")
                            .Append(Settings.PrettyPrint ? Environment.NewLine : string.Empty)
                            .Append($"ORDER BY {sbOrderBy}");

                }


                if (query is SelectQuery)
                {
                    SelectQuery selectQuery = (SelectQuery)query;

                    if (selectQuery.Unions != null)
                    {
                        foreach (IUnionQuery<SelectQuery> unionQuery in selectQuery.Unions)
                        {
                            SelectQuery union = unionQuery.Build();
                            sb
                                .Append(' ')
                                .Append(Settings.PrettyPrint ? Environment.NewLine : string.Empty)
                                .Append("UNION ")
                                .Append(Settings.PrettyPrint ? Environment.NewLine : string.Empty)
                                .Append(Render(union.Build()));
                        }
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
                foreach (IJoin @join in joins)
                {
                    if (sbJoins.Length > 0)
                    {
                        sbJoins = sbJoins.Append(" ");
                    }

                    switch (@join.JoinType)
                    {
                        case JoinType.CrossJoin:
                            sbJoins = sbJoins.Append($"CROSS JOIN { RenderTablename(@join.Table, renderAlias: true)}");
                            break;
                        case JoinType.LeftOuterJoin:
                            sbJoins = sbJoins.Append($"LEFT OUTER JOIN {RenderTablename(@join.Table, renderAlias: true)} ON {RenderWhere(@join.On)}");
                            break;
                        case JoinType.RightOuterJoin:
                            sbJoins = sbJoins.AppendFormat($"RIGHT OUTER JOIN {RenderTablename(@join.Table, renderAlias: true)} ON {RenderWhere(@join.On)}");
                            break;
                        case JoinType.InnerJoin:
                            sbJoins = sbJoins.AppendFormat($"INNER JOIN {RenderTablename(@join.Table, renderAlias: true)} ON {RenderWhere(@join.On)}");
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
            foreach (ITable item in tables)
            {
                if (sbTables.Length != 0)
                {
                    sbTables = sbTables.Append(", ");
                }

                if (item is Table table)
                {
                    sbTables = string.IsNullOrWhiteSpace(table.Alias)
                        ? sbTables.Append(EscapeName(table.Name))
                        : sbTables.Append(RenderTablenameWithAlias(EscapeName(table.Name), EscapeName(table.Alias)));
                }
                else if (item is SelectQuery selectTable)
                {
                    sbTables = string.IsNullOrWhiteSpace(selectTable.Alias)
                        ? sbTables.Append($"({Render(selectTable)})")
                        : sbTables.Append($"({Render(selectTable)}) {EscapeName(selectTable.Alias)}");
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
                case ClauseOperator.In:
                    switch (clause.Constraint)
                    {
                        case VariableValues variables:
                            clauseString = $"{RenderColumn(clause.Column, false)} IN ({string.Join(", ", variables.Select(x => RenderVariable(x, false)))})";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(clause), $"Unsupported constraint type");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return clauseString;
        }

        protected virtual string RenderWhere(IWhereClause clause)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (clause is WhereClause whereClause)
            {
                sbWhere.Append(RenderClause(whereClause));
            }
            else if (clause is CompositeWhereClause compositeClause)
            {
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
                            sbWhere = sbWhere.Append(RenderWhere(innerClause));
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

            if (clause is HavingClause havingClause)
            {
                sbHaving.Append(RenderClause(havingClause));
            }
            else if (clause is CompositeHavingClause compositeClause)
            {
                switch (compositeClause.Logic)
                {
                    case ClauseLogic.And:
                        foreach (IHavingClause innerClause in compositeClause.Clauses)
                        {
                            if (sbHaving.Length > 0)
                            {
                                sbHaving = sbHaving.Append(" AND ");
                            }
                            sbHaving = sbHaving.Append(RenderHaving(innerClause));
                        }

                        break;
                    case ClauseLogic.Or:
                        foreach (IHavingClause innerClause in compositeClause.Clauses)
                        {
                            if (sbHaving.Length > 0)
                            {
                                sbHaving = sbHaving.Append(" OR ");
                            }
                            sbHaving = sbHaving.Append(RenderHaving(innerClause));
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return sbHaving.Insert(0, '(').Insert(sbHaving.Length, ')').ToString();
        }

        protected virtual string RenderTablename(Table table, bool renderAlias)
            => !renderAlias || string.IsNullOrWhiteSpace(table.Alias)
                ? EscapeName(table.Name)
                : $"{EscapeName(table.Name)} {EscapeName(table.Alias)}";

        /// <summary>
        /// Renders the column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="renderAlias"><code>true</code> to render the alias associated with the column</param>
        /// <returns></returns>
        protected virtual string RenderColumn(IColumn column, bool renderAlias)
        {
            string columnString = string.Empty;
            if (column == null)
            {
                columnString = "NULL";
            }
            else if (column.GetType().GetTypeInfo().GetCustomAttribute<FunctionAttribute>() != null)
            {
                columnString = RenderFunction(column, renderAlias);
            }
            else
            {
                switch (column)
                {
                    case FieldColumn fieldColumn:
                        columnString = !renderAlias || string.IsNullOrWhiteSpace(fieldColumn.Alias)
                    ? EscapeName(fieldColumn.Name)
                    : RenderColumnnameWithAlias(EscapeName(fieldColumn.Name), EscapeName(fieldColumn.Alias));
                        break;
                    case LiteralColumn literalColumn:
                        columnString = RenderLiteralColumn(literalColumn, renderAlias);
                        break;
                    case SelectColumn selectColumn:
                        columnString = RenderInlineSelect(selectColumn, renderAlias);
                        break;
                    case UniqueIdentifierValue _:
                        columnString = RenderUUIDValue();
                        break;

                    case Variable variable:
                        columnString = RenderVariable(variable, renderAlias);
                        break;
                }
            }

            return columnString;
        }

        protected virtual string RenderVariable(Variable variable, bool renderAlias) => throw new NotSupportedException();

        protected virtual string RenderUUIDValue() => $"NEWID()";

        protected virtual string RenderFunction(IColumn column, bool renderAlias)
        {
            string columnString = string.Empty;
            switch (column)
            {
                case AggregateFunction aggregateColumn:
                    columnString = RenderAggregateColumn(aggregateColumn, renderAlias);
                    break;
                case ConcatFunction concatColumn:
                    columnString = RenderConcatColumn(concatColumn, renderAlias);
                    break;
                case NullFunction nullColumn:
                    columnString = RenderNullColumn(nullColumn, renderAlias);
                    break;
                case LengthFunction lengthColumn:
                    columnString = RenderLengthColumn(lengthColumn, renderAlias);
                    break;
                case SubstringFunction substringColumn:
                    columnString = RenderSubstringColumn(substringColumn, renderAlias);
                    break;
                case UpperFunction upperColumn:
                    columnString = RenderUpperColumn(upperColumn, renderAlias);
                    break;
            }

            return columnString;
        }

        /// <summary>
        /// Gets the name of the "LENGTH" function
        /// </summary>
        /// <returns></returns>
        protected virtual string LengthFunctionName => "LENGTH";

        /// <summary>
        /// Gets the name of the "SUBSTRING" function
        /// </summary>
        protected virtual string SubstringFunctionName => "SUBSTRING";

        /// <summary>
        /// Gets the name of the "UPPER" function
        /// </summary>
        protected virtual string UpperFunctionName => "UPPER";

        protected virtual string RenderLengthColumn(LengthFunction lengthColumn, bool renderAlias)
        {
            string sbLengthColumn = $"{LengthFunctionName}({RenderColumn(lengthColumn.Column, false)})";

            string queryString = renderAlias && !string.IsNullOrWhiteSpace(lengthColumn.Alias)
                ? RenderColumnnameWithAlias(sbLengthColumn, EscapeName(lengthColumn.Alias))
                : sbLengthColumn;

            return queryString;
        }

        protected virtual string RenderSubstringColumn(SubstringFunction substringColumn, bool renderAlias)
        {
            string sbLengthColumn = $"{SubstringFunctionName}({RenderColumn(substringColumn.Column, false)}, {substringColumn.Start}{(substringColumn.Length.HasValue ? $", {substringColumn.Length.Value}" : "")})";

            string queryString = renderAlias && !string.IsNullOrWhiteSpace(substringColumn.Alias)
                ? RenderColumnnameWithAlias(sbLengthColumn, EscapeName(substringColumn.Alias))
                : sbLengthColumn;

            return queryString;
        }

        protected virtual string RenderUpperColumn(UpperFunction upperColumn, bool renderAlias)
        {
            string sbLengthColumn = $"{UpperFunctionName}({RenderColumn(upperColumn.Column, false)})";

            string queryString = renderAlias && !string.IsNullOrWhiteSpace(upperColumn.Alias)
                ? RenderColumnnameWithAlias(sbLengthColumn, EscapeName(upperColumn.Alias))
                : sbLengthColumn;

            return queryString;
        }

        protected virtual string RenderConcatColumn(ConcatFunction concatColumn, bool renderAlias)
        {
            StringBuilder sbConcat = new StringBuilder();
            foreach (IColumn column in concatColumn.Columns)
            {
                if (sbConcat.Length > 0)
                {
                    sbConcat = sbConcat.Append($" {ConcatOperator} ");
                }
                sbConcat = sbConcat.Append(RenderColumn(column, renderAlias: false));
            }

            string queryString = renderAlias && !string.IsNullOrWhiteSpace(concatColumn.Alias)
                ? RenderColumnnameWithAlias(sbConcat.ToString(), EscapeName(concatColumn.Alias))
                : sbConcat.ToString();

            return queryString;

        }

        protected virtual string RenderNullColumn(NullFunction nullColumn, bool renderAlias)
        {
            string sbNullColumn = $"ISNULL({RenderColumn(nullColumn.Column, false)}, {RenderColumn(nullColumn.DefaultValue, false)})";

            string queryString = renderAlias && !string.IsNullOrWhiteSpace(nullColumn.Alias)
                ? RenderColumnnameWithAlias(sbNullColumn, EscapeName(nullColumn.Alias))
                : sbNullColumn;

            return queryString;
        }

        /// <summary>
        /// Gets the string that can be used to start escaping a reserved word
        /// </summary>
        /// <returns></returns>
        protected abstract string BeginEscapeWordString { get; }

        /// <summary>
        /// Gets the string that can be used to indicate the word was escaped properly
        /// </summary>
        /// <returns></returns>
        protected abstract string EndEscapeWordString { get; }


        /// <summary>
        /// Gets the operator used to concatenate values
        /// </summary>
        /// <returns></returns>
        protected abstract string ConcatOperator { get; }

        protected virtual string RenderInlineSelect(SelectColumn inlineSelectQuery, bool renderAlias)
        {
            string columnString = !renderAlias || string.IsNullOrWhiteSpace(inlineSelectQuery.Alias)
                        ? $"({Render(inlineSelectQuery.SelectQuery)})"
                : RenderColumnnameWithAlias($"({Render(inlineSelectQuery.SelectQuery)})", EscapeName(inlineSelectQuery.Alias));
            return columnString;
        }

        protected virtual string RenderTablenameWithAlias(string tableName, string alias) => $"{tableName} {alias}";

        protected virtual string RenderColumnnameWithAlias(string columnName, string alias) => $"{columnName} AS {alias}";

        protected virtual string RenderAggregateColumn(AggregateFunction ac, bool renderAlias)
        {
            string columnString;
            switch (ac.Type)
            {
                case AggregateType.Min:
                    columnString = !renderAlias || string.IsNullOrWhiteSpace(ac.Alias)
                        ? $"MIN({RenderColumn(ac.Column, renderAlias: false)})"
                        : RenderColumnnameWithAlias($"MIN({RenderColumn(ac.Column, renderAlias: false)})", EscapeName(ac.Alias));
                    break;
                case AggregateType.Max:
                    columnString = !renderAlias || string.IsNullOrWhiteSpace(ac.Alias)
                        ? $"MAX({RenderColumn(ac.Column, renderAlias: false)})"
                        : RenderColumnnameWithAlias($"MAX({RenderColumn(ac.Column, renderAlias: false)})", EscapeName(ac.Alias));
                    break;
                case AggregateType.Average:
                    columnString = !renderAlias || string.IsNullOrWhiteSpace(ac.Alias)
                        ? $"AVG({RenderColumn(ac.Column, renderAlias: false)})"
                        : RenderColumnnameWithAlias($"AVG({RenderColumn(ac.Column, renderAlias: false)})", EscapeName(ac.Alias));
                    break;
                case AggregateType.Count:
                    columnString = !renderAlias || string.IsNullOrWhiteSpace(ac.Alias)
                        ? $"COUNT({RenderColumn(ac.Column, renderAlias: false)})"
                        : RenderColumnnameWithAlias($"COUNT({RenderColumn(ac.Column, renderAlias: false)})", EscapeName(ac.Alias));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return columnString;
        }

        protected virtual string RenderLiteralColumn<T>(T lc, bool renderAlias) where T : LiteralColumn
        {
            string columnString;

            object value = lc.Value;
            string stringValue = value.ToString();
            switch (lc)
            {
                case StringColumn sc:
                    columnString = renderAlias && !string.IsNullOrWhiteSpace(sc.Alias)
                    ? $"'{EscapeString(stringValue)}' AS {EscapeName(sc.Alias)}"
                    : $"'{EscapeString(stringValue)}'";
                    break;
                case DateTimeColumn dc:
                    columnString = renderAlias && !string.IsNullOrWhiteSpace(dc.Alias)
                    ? $"'{EscapeString((dc.Value as DateTime?)?.ToString(Settings.DateFormatString))}' AS {EscapeName(dc.Alias)}"
                    : $"'{EscapeString((dc.Value as DateTime?)?.ToString(Settings.DateFormatString))}'";
                    break;
                default:
                    columnString = renderAlias && !string.IsNullOrWhiteSpace(lc.Alias)
                    ? $"{value} AS {EscapeName(lc.Alias)}"
                    : value.ToString();
                    break;
            }

            return columnString;

        }

        /// <summary>
        /// Escape a <see cref="string"/> value so that it can be safely used in a string query
        /// </summary>
        /// <param name="unescapedString">the value to escape.</param>
        /// <returns>a safe <see cref="string"/></returns>
        protected virtual string EscapeString(string unescapedString) => unescapedString?.Replace("'", "''");

        protected virtual string Render(UpdateQuery updateQuery)
        {
            StringBuilder queryStringBuilder = new StringBuilder();

            if (updateQuery != null)
            {
                StringBuilder sbFieldsToUpdate = new StringBuilder();
                foreach (UpdateFieldValue queryFieldValue in updateQuery.Values)
                {
                    if (sbFieldsToUpdate.Length > 0)
                    {
                        sbFieldsToUpdate = sbFieldsToUpdate
                            .Append(", ")
                            .Append(Settings.PrettyPrint ? Environment.NewLine : string.Empty);
                    }

                    sbFieldsToUpdate = sbFieldsToUpdate.Append($"{RenderColumn(queryFieldValue.Destination, false)} = {RenderColumn(queryFieldValue.Source, false)}");
                }

                queryStringBuilder = queryStringBuilder
                    .AppendFormat("UPDATE {0} ", RenderTablename(updateQuery.Table, renderAlias: false))
                    .Append(Settings.PrettyPrint ? Environment.NewLine : string.Empty)
                    .AppendFormat("SET {0}", sbFieldsToUpdate);

                if (updateQuery.Criteria != null)
                {
                    queryStringBuilder = queryStringBuilder
                        .Append(" ")
                        .Append(Settings.PrettyPrint ? Environment.NewLine : string.Empty)
                        .AppendFormat("WHERE {0}", RenderWhere(updateQuery.Criteria));
                }

            }


            return queryStringBuilder.ToString();
        }


        protected virtual string Render(CreateViewQuery query)
        {
            StringBuilder sb = new StringBuilder();

            sb = sb.AppendFormat("CREATE VIEW {0} ", RenderTables(new ITable[] { query.ViewName.Table() }))

                .Append(Settings.PrettyPrint ? Environment.NewLine : string.Empty)
                .AppendFormat("AS ")
                .Append(Settings.PrettyPrint ? Environment.NewLine : string.Empty)
                .Append(Render(query.SelectQuery));


            return sb.ToString();
        }

        protected virtual string Render(TruncateQuery query)
        {
            string sbQuery = string.Empty;
            if (query != null)
            {
                sbQuery = $"TRUNCATE TABLE {BeginEscapeWordString}{query.Name}{EndEscapeWordString}";
            }

            return sbQuery;
        }

        protected virtual string Render(DeleteQuery deleteQuery)
        {


            StringBuilder sbQuery = new StringBuilder();

            if (deleteQuery != null)
            {
                sbQuery = sbQuery.Append($"DELETE FROM {EscapeName(deleteQuery.Table)}");

                if (deleteQuery.Criteria != null)
                {
                    sbQuery = sbQuery
                        .Append($" {(Settings.PrettyPrint ? Environment.NewLine : string.Empty)}WHERE {RenderWhere(deleteQuery.Criteria)}");
                }
            }


            return sbQuery.ToString();

        }

        /// <summary>
        /// Renders the batch queries
        /// </summary>
        /// <param name="query">The batch query to execute</param>
        /// <returns></returns>
        protected virtual string Render(BatchQuery query)
        {
            StringBuilder sbResult = new StringBuilder();

            IEnumerable<IQuery> statements = query.Statements?.ToArray() ?? Enumerable.Empty<IQuery>().ToArray();
            if (statements.Any())
            {
                IQuery currentStatement = statements.First();
                sbResult.Append(Render(currentStatement));
                IQuery previousStatement = currentStatement;
                for (int i = 1; i < statements.Count(); i++)
                {
                    currentStatement = statements.ElementAt(i);
                    if (isDataManipulationQuery(previousStatement))
                    {
                        sbResult.AppendLine(BatchStatementSeparator);
                    }
                    sbResult.Append(Render(currentStatement));
                    previousStatement = currentStatement;
                }

            }
            bool isDataManipulationQuery(IQuery q) => q.GetType()
                .GetTypeInfo()
                .GetCustomAttribute<DataManipulationLanguageAttribute>() != null;

            return sbResult.ToString();
        }

        public virtual string BatchStatementSeparator => ";";
    }
}