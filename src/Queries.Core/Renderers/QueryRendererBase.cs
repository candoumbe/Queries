using Queries.Core.Attributes;
using Queries.Core.Builders;
using Queries.Core.Builders.Fluent;
using Queries.Core.Parts;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;
using Queries.Core.Parts.Joins;
using Queries.Core.Parts.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Queries.Core.Renderers
{
    using static PaginationKind;

    /// <summary>
    /// Base class for query renderers
    /// </summary>
    public abstract class QueryRendererBase : IQueryRenderer
    {
        /// <summary>
        /// Settings currently used to render <see cref="IQuery"/> instances
        /// </summary>
        public QueryRendererSettings Settings { get; }

        /// <summary>
        /// Builds a new <see cref="QueryRendererBase"/> instance.
        /// 
        /// </summary>
        /// <param name="settings"><see cref="QueryRendererSettings"/> used to render <see cref="IQuery"/> instances</param>
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
            => query switch
            {
                SelectQueryBase selectQueryBase => Render(selectQueryBase),
                CreateViewQuery createViewQuery => Render(createViewQuery),
                DeleteQuery deleteQuery => Render(deleteQuery),
                UpdateQuery updateQuery => Render(updateQuery),
                TruncateQuery truncateQuery => Render(truncateQuery),
                InsertIntoQuery insertIntoQuery => Render(insertIntoQuery),
                BatchQuery batchQuery => Render(batchQuery),
                NativeQuery nativeQuery => nativeQuery.Statement,
                _ => throw new ArgumentOutOfRangeException("Unsupported query type"),
            };

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
                if ((query.Columns?.Count ?? 0) > 0)
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

                        if (limit.HasValue && (Settings.PaginationKind & Top) == Top)
                        {
                            sb.Append($"SELECT TOP {limit.Value} {fieldsString} ");
                        }
                        else
                        {
                            sb.AppendFormat("SELECT {0} ", fieldsString);
                        }
                        sb.Append(Settings.PrettyPrint ? Environment.NewLine : string.Empty)
                            .AppendFormat("FROM {0}", tableString);
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

                    foreach (IOrder sort in query.Sorts)
                    {
                        if (sbOrderBy.Length > 0)
                        {
                            sbOrderBy = sbOrderBy.Append(", ");
                        }

                        sbOrderBy = sort.Direction == OrderDirection.Descending
                            ? sbOrderBy.Append($"{RenderColumn(sort.Column, false)} DESC")
                            : sbOrderBy.Append(RenderColumn(sort.Column, false));
                    }
                    sb.Append(" ")
                            .Append(Settings.PrettyPrint ? Environment.NewLine : string.Empty)
                            .Append($"ORDER BY {sbOrderBy}");
                }

                if (query is SelectQuery selectQuery2)
                {
                    SelectQuery selectQuery = selectQuery2;

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

                    if ((Settings.PaginationKind & Limit) == Limit && selectQuery2.NbRows.HasValue)
                    {
                        sb.Append(' ')
                            .Append("LIMIT ")
                            .Append(selectQuery2.NbRows);
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

                    sbJoins = @join.JoinType switch
                    {
                        JoinType.CrossJoin => sbJoins.Append($"CROSS JOIN { RenderTablename(@join.Table, renderAlias: true)}"),
                        JoinType.LeftOuterJoin => sbJoins.Append($"LEFT OUTER JOIN {RenderTablename(@join.Table, renderAlias: true)} ON {RenderWhere(@join.On)}"),
                        JoinType.RightOuterJoin => sbJoins.AppendFormat($"RIGHT OUTER JOIN {RenderTablename(@join.Table, renderAlias: true)} ON {RenderWhere(@join.On)}"),
                        JoinType.InnerJoin => sbJoins.AppendFormat($"INNER JOIN {RenderTablename(@join.Table, renderAlias: true)} ON {RenderWhere(@join.On)}"),
                        _ => throw new ArgumentOutOfRangeException(),
                    };
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
                if (sbFields.Length > 0)
                {
                    sbFields = sbFields.Append(", ");
                }
                sbFields.Append(RenderColumn(column, renderAlias: true));
            }

            return sbFields.ToString();
        }

        protected virtual string RenderClause<T>(IClause<T> clause) where T : IColumn
            => clause.Operator switch
            {
                ClauseOperator.EqualTo => $"{RenderColumn(clause.Column, false)} = {RenderColumn(clause.Constraint, false)}",
                ClauseOperator.NotEqualTo => $"{RenderColumn(clause.Column, false)} <> {RenderColumn(clause.Constraint, false)}",
                ClauseOperator.LessThan => $"{RenderColumn(clause.Column, false)} < {RenderColumn(clause.Constraint, false)}",
                ClauseOperator.GreaterThan => $"{RenderColumn(clause.Column, false)} > {RenderColumn(clause.Constraint, false)}",
                ClauseOperator.Like => $"{RenderColumn(clause.Column, false)} LIKE {RenderColumn(clause.Constraint, false)}",
                ClauseOperator.NotLike => $"{RenderColumn(clause.Column, false)} NOT LIKE {RenderColumn(clause.Constraint, false)}",
                ClauseOperator.LessThanOrEqualTo => $"{RenderColumn(clause.Column, false)} <= {RenderColumn(clause.Constraint, false)}",
                ClauseOperator.GreaterThanOrEqualTo => $"{RenderColumn(clause.Column, false)} >= {RenderColumn(clause.Constraint, false)}",
                ClauseOperator.IsNull => $"{RenderColumn(clause.Column, false)} IS NULL",
                ClauseOperator.IsNotNull => $"{RenderColumn(clause.Column, false)} IS NOT NULL",
                ClauseOperator.In => clause.Constraint switch
                {
                    VariableValues variables => $"{RenderColumn(clause.Column, false)} IN ({string.Join(", ", variables.Select(x => RenderVariable(x, false)))})",
                    _ => throw new ArgumentOutOfRangeException(nameof(clause), $"Unsupported constraint type for {nameof(ClauseOperator)}.{nameof(ClauseOperator.In)} operator."),
                },
                ClauseOperator.NotIn => clause.Constraint switch
                {
                    VariableValues variables => $"{RenderColumn(clause.Column, false)} NOT IN ({string.Join(", ", variables.Select(x => RenderVariable(x, false)))})",
                    _ => throw new ArgumentOutOfRangeException(nameof(clause), $"Unsupported constraint type for {nameof(ClauseOperator)}.{nameof(ClauseOperator.NotIn)} operator."),
                },
                _ => throw new ArgumentOutOfRangeException(nameof(clause.Operator), "Unknown clause operator"),
            };

        protected virtual string RenderWhere(IWhereClause clause)
        {
            StringBuilder sbWhere = new StringBuilder();

            switch (clause)
            {
                case WhereClause whereClause:
                    sbWhere.Append(RenderClause(whereClause));
                    break;
                case CompositeWhereClause compositeClause:
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
                            throw new ArgumentOutOfRangeException(nameof(compositeClause.Logic), "Unknown logic");
                    }

                    break;
            }

            return sbWhere.Insert(0, '(').Insert(sbWhere.Length, ')').ToString();
        }

        protected virtual string RenderHaving(IHavingClause clause)
        {
            StringBuilder sbHaving = new StringBuilder();

            switch (clause)
            {
                case HavingClause havingClause:
                    sbHaving.Append(RenderClause(havingClause));
                    break;
                case CompositeHavingClause compositeClause:
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

                    break;
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
            string columnString;
            if (column is null)
            {
                columnString = "NULL";
            }
            else if (column.GetType().GetTypeInfo().GetCustomAttribute<FunctionAttribute>() != null)
            {
                columnString = RenderFunction(column, renderAlias);
            }
            else
            {
                columnString = column switch
                {
                    FieldColumn fieldColumn => !renderAlias || string.IsNullOrWhiteSpace(fieldColumn.Alias)
                        ? EscapeName(fieldColumn.Name)
                        : RenderColumnnameWithAlias(EscapeName(fieldColumn.Name), EscapeName(fieldColumn.Alias)),
                    Literal literalColumn => RenderLiteralColumn(literalColumn, renderAlias),
                    SelectColumn selectColumn => RenderInlineSelect(selectColumn, renderAlias),
                    UniqueIdentifierValue _ => RenderUUIDValue(),
                    Variable variable => RenderVariable(variable, renderAlias),
                    SelectQuery select => RenderInlineSelect(select, renderAlias),
                    _ => throw new ArgumentOutOfRangeException($"Unhandled {column?.GetType()} rendering as column"),
                };
            }

            return columnString;
        }

        protected virtual string RenderVariable(Variable variable, bool renderAlias) => throw new NotSupportedException();

        protected virtual string RenderUUIDValue() => $"NEWID()";

        protected virtual string RenderFunction(IColumn column, bool renderAlias)
            => column switch
            {
                AggregateFunction aggregateColumn => RenderAggregateColumn(aggregateColumn, renderAlias),
                ConcatFunction concatColumn => RenderConcatColumn(concatColumn, renderAlias),
                NullFunction nullColumn => RenderNullColumn(nullColumn, renderAlias),
                LengthFunction lengthColumn => RenderLengthColumn(lengthColumn, renderAlias),
                SubstringFunction substringColumn => RenderSubstringColumn(substringColumn, renderAlias),
                UpperFunction upperColumn => RenderUpperColumn(upperColumn, renderAlias),
                _ => throw new ArgumentOutOfRangeException("Unknown function type"),
            };

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

            return renderAlias && !string.IsNullOrWhiteSpace(lengthColumn.Alias)
                ? RenderColumnnameWithAlias(sbLengthColumn, EscapeName(lengthColumn.Alias))
                : sbLengthColumn;
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

            return renderAlias && !string.IsNullOrWhiteSpace(upperColumn.Alias)
                ? RenderColumnnameWithAlias(sbLengthColumn, EscapeName(upperColumn.Alias))
                : sbLengthColumn;
        }

        protected virtual string RenderConcatColumn(ConcatFunction concatColumn, bool renderAlias)
        {
            StringBuilder sbConcat = new StringBuilder();
            foreach (IColumn column in concatColumn.Columns)
            {
                if (sbConcat.Length > 0)
                {
                    sbConcat = sbConcat.Append(" ").Append(ConcatOperator).Append(" ");
                }
                sbConcat = sbConcat.Append(RenderColumn(column, renderAlias: false));
            }

            return renderAlias && !string.IsNullOrWhiteSpace(concatColumn.Alias)
                ? RenderColumnnameWithAlias(sbConcat.ToString(), EscapeName(concatColumn.Alias))
                : sbConcat.ToString();
        }

        protected virtual string RenderNullColumn(NullFunction nullColumn, bool renderAlias)
        {
            string sbNullColumn = $"ISNULL({RenderColumn(nullColumn.Column, false)}, {RenderColumn(nullColumn.DefaultValue, false)})";

            return renderAlias && !string.IsNullOrWhiteSpace(nullColumn.Alias)
                ? RenderColumnnameWithAlias(sbNullColumn, EscapeName(nullColumn.Alias))
                : sbNullColumn;
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
            => !renderAlias || string.IsNullOrWhiteSpace(inlineSelectQuery.Alias)
                ? $"({Render(inlineSelectQuery.SelectQuery)})"
                : RenderColumnnameWithAlias($"({Render(inlineSelectQuery.SelectQuery)})", EscapeName(inlineSelectQuery.Alias));

        protected virtual string RenderTablenameWithAlias(string tableName, string alias) => $"{tableName} {alias}";

        protected virtual string RenderColumnnameWithAlias(string columnName, string alias) => $"{columnName} AS {alias}";

        protected virtual string RenderAggregateColumn(AggregateFunction ac, bool renderAlias)
            => ac.Type switch
            {
                AggregateType.Min => !renderAlias || string.IsNullOrWhiteSpace(ac.Alias)
                        ? $"MIN({RenderColumn(ac.Column, renderAlias: false)})"
                        : RenderColumnnameWithAlias($"MIN({RenderColumn(ac.Column, renderAlias: false)})", EscapeName(ac.Alias)),
                AggregateType.Max => !renderAlias || string.IsNullOrWhiteSpace(ac.Alias)
                        ? $"MAX({RenderColumn(ac.Column, renderAlias: false)})"
                        : RenderColumnnameWithAlias($"MAX({RenderColumn(ac.Column, renderAlias: false)})", EscapeName(ac.Alias)),
                AggregateType.Average => !renderAlias || string.IsNullOrWhiteSpace(ac.Alias)
                        ? $"AVG({RenderColumn(ac.Column, renderAlias: false)})"
                        : RenderColumnnameWithAlias($"AVG({RenderColumn(ac.Column, renderAlias: false)})", EscapeName(ac.Alias)),
                AggregateType.Count => !renderAlias || string.IsNullOrWhiteSpace(ac.Alias)
                        ? $"COUNT({RenderColumn(ac.Column, renderAlias: false)})"
                        : RenderColumnnameWithAlias($"COUNT({RenderColumn(ac.Column, renderAlias: false)})", EscapeName(ac.Alias)),
                _ => throw new ArgumentOutOfRangeException(),
            };

        protected virtual string RenderLiteralColumn<T>(T lc, bool renderAlias) where T : Literal
        {
            object value = lc.Value;
            string stringValue = value.ToString();
            return lc switch
            {
                StringColumn sc => renderAlias && !string.IsNullOrWhiteSpace(sc.Alias)
                   ? $"'{EscapeString(stringValue)}' AS {EscapeName(sc.Alias)}"
                   : $"'{EscapeString(stringValue)}'",
                DateTimeColumn dc => renderAlias && !string.IsNullOrWhiteSpace(dc.Alias)
                    ? $"'{EscapeString((dc.Value as DateTime?)?.ToString(Settings.DateFormatString))}' AS {EscapeName(dc.Alias)}"
                    : $"'{EscapeString((dc.Value as DateTime?)?.ToString(Settings.DateFormatString))}'",
                _ => renderAlias && !string.IsNullOrWhiteSpace(lc.Alias)
                    ? $"{value} AS {EscapeName(lc.Alias)}"
                    : value.ToString(),
            };
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

            IEnumerable<IQuery> statements = query.Statements?.ToArray() ?? Enumerable.Empty<IQuery>();
            if (statements.Any())
            {
                foreach (IQuery statement in statements)
                {
                    sbResult
                        .Append(Render(statement))
                        .Append(BatchStatementSeparator);
                }
            }

            return sbResult.ToString();
        }

        public virtual string BatchStatementSeparator => ";";

        public CompiledQuery Compile(IQuery query)
        {
            CollectVariableVisitor visitor = new CollectVariableVisitor();
            CompiledQuery compiledQuery;
            switch (query)
            {
                case SelectQuery selectQuery:
                    visitor.Visit(selectQuery);

                    compiledQuery = new CompiledQuery(Render(selectQuery), visitor.Variables);
                    break;
                default:
                    compiledQuery = new CompiledQuery(Render(query), Enumerable.Empty<Variable>());
                    break;
            }

            return compiledQuery;
        }
    }
}