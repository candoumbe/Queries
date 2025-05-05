using Queries.Core.Attributes;
using Queries.Core.Builders;
using Queries.Core.Builders.Fluent;
using Queries.Core.Parts;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;
using Queries.Core.Parts.Functions.Math;
using Queries.Core.Parts.Joins;
using Queries.Core.Parts.Sorting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using static Queries.Core.Renderers.PaginationKind;

namespace Queries.Core.Renderers
{
    /// <summary>
    /// Base class for query renderers
    /// </summary>
    public abstract class QueryRendererBase : IQueryRenderer
    {
        private const string LeftParenthesis = "(";
        private const string RightParenthesis = ")";

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

            if (rawName is not null)
            {
                string[] rawNameParts = rawName.Split('.');
                StringBuilder sb = new();

                foreach (string namePart in rawNameParts)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append('.');
                    }

                    sb.Append("*".Equals(namePart.Trim())
                        ? "*"
                        : $"{BeginEscapeWordString}{namePart}{EndEscapeWordString}");
                }

                escapedColumnName = sb.ToString();
            }

            return escapedColumnName;
        }

        /// <inheritdoc />
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
                _ => throw new ArgumentOutOfRangeException(nameof(query), query, $"Unexpected {query.GetType()} query type"),
            };

        /// <summary>
        /// Renders the specified <see cref="InsertIntoQuery"/> query.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
        protected virtual string Render(InsertIntoQuery query)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query), $"{nameof(query)} cannot be null");
            }

            string queryString = string.Empty;

            if (query.InsertedValue is SelectQuery selectQuery)
            {
                queryString = $"INSERT INTO {RenderTablename(query.TableName.Table(), renderAlias: false)} {(Settings.PrettyPrint ? Environment.NewLine : string.Empty)}" +
                    Render(selectQuery);
            }
            else if (query.InsertedValue is IEnumerable<InsertedValue> values)
            {
                StringBuilder sbColumns = new();
                StringBuilder sbValues = new();
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

        /// <summary>
        /// Renders the specified
        /// </summary>
        /// <param name="query"></param>
        /// <param name="blockLevel"></param>
        /// <returns></returns>
        protected virtual string Render(SelectQueryBase query, int blockLevel = 0)
        {
            QueryWriter writer = new(blockLevel, prettyPrint: Settings.PrettyPrint);

            if (query is null)
            {
                return writer.Value;
            }

            string fieldsString = (query.Columns?.Count ?? 0) > 0
                ? RenderColumns(query.Columns, writer.BlockLevel)
                : "*";

            switch (query)
            {
                case SelectQuery selectQuery:
                    if (selectQuery.Tables.AtLeastOnce())
                    {
                        int? pageSize = selectQuery.PageSize;
                        int? pageIndex = selectQuery.PageIndex;

                        if ((pageIndex ?? 1) == 1 && pageSize >= 1 && (Settings.PaginationKind & Top) == Top)
                        {
                            writer.WriteText($"SELECT TOP {pageSize.Value}");
                        }
                        else
                        {
                            writer.WriteText("SELECT");
                        }

                        writer.StartBlock();
                        writer.WriteText(fieldsString);
                        writer.EndBlock();

                        writer.WriteText("FROM");

                        string tableString = RenderTables(selectQuery.Tables.ToArray(), 0);
                        writer.StartBlock();
                        writer.WriteText(tableString);
                        writer.EndBlock();
                    }
                    else
                    {
                        writer.WriteText($"SELECT {fieldsString}");
                    }

                    break;

                case SelectIntoQuery selectInto:

                    writer.WriteText("SELECT");

                    writer.StartBlock();
                    writer.WriteText(fieldsString);
                    writer.EndBlock();

                    writer.WriteText("INTO");

                    writer.StartBlock();
                    writer.WriteText(RenderTablename(selectInto.Destination, false));
                    writer.EndBlock();

                    writer.WriteText("FROM");
                    writer.WriteText(RenderTables([selectInto.Source], writer.BlockLevel));

                    break;
            }

            if (query.Joins.AtLeastOnce())
            {
                string joinString = RenderJoins(query.Joins);
                writer.WriteText(joinString);
            }

            if (query.WhereCriteria is not null)
            {
                writer.WriteText($"WHERE {RenderWhere(query.WhereCriteria)}");
            }

            if (query.Columns is not null)
            {
                IEnumerable<AggregateFunction> aggregatedColumns = query.Columns.OfType<AggregateFunction>();
                IEnumerable<FieldColumn> tableColumns = query.Columns.OfType<FieldColumn>();

                if (aggregatedColumns.AtLeastOnce() && tableColumns.AtLeastOnce())
                {
                    StringBuilder sbGroupBy = new();
                    foreach (FieldColumn column in query.Columns.OfType<FieldColumn>())
                    {
                        if (sbGroupBy.Length > 0)
                        {
                            sbGroupBy = sbGroupBy.Append(", ");
                        }
                        sbGroupBy = sbGroupBy.Append(EscapeName(column.Name));
                    }

                    // Automagically adds a GROUP BY when needed
                    writer.WriteText($"GROUP BY {sbGroupBy}");
                }
            }

            if (query.HavingCriteria is not null)
            {
                writer.WriteText($"HAVING {RenderHaving(query.HavingCriteria)}");
            }

            if (query.Orders.AtLeastOnce())
            {
                StringBuilder sbOrderBy = new();

                foreach (IOrder sort in query.Orders)
                {
                    if (sbOrderBy.Length > 0)
                    {
                        sbOrderBy = sbOrderBy.Append(", ");
                    }

                    sbOrderBy = sort.Direction == OrderDirection.Descending
                        ? sbOrderBy.Append($"{RenderColumn(sort.Column, false)} DESC")
                        : sbOrderBy.Append(RenderColumn(sort.Column, false));
                }

                writer.WriteText("ORDER BY");

                writer.StartBlock();
                writer.WriteText(sbOrderBy.ToString());
                writer.EndBlock();
            }

            if (query is SelectQuery sq)
            {
                SelectQuery selectQuery = sq;

                if (selectQuery.Unions is not null)
                {
                    foreach (IUnionQuery<SelectQuery> unionQuery in selectQuery.Unions)
                    {
                        SelectQuery union = unionQuery.Build();

                        writer.WriteText("UNION");
                        writer.WriteText(Render(union.Build(), blockLevel));
                    }
                }

                int? pageSize = sq.PageSize;
                int? pageIndex = sq.PageIndex;

                if (pageIndex >= 2 && pageSize > 0)
                {
                    writer.StartBlock();
                    writer.WriteText(RenderPagination(pageIndex.Value, pageSize.Value));
                    writer.EndBlock();
                }

                if ((Settings.PaginationKind & Limit) == Limit && sq.PageSize.HasValue && (sq.PageIndex ?? 1) == 1)
                {
                    writer.WriteText($"LIMIT {sq.PageSize}");
                }
            }

            return writer.Value;
        }

        ///<inheritdoc/>
        protected virtual string RenderPagination(int pageIndex, int pageSize)
        {
            StringBuilder sb = new StringBuilder()
                .Append("OFFSET ").Append(pageSize);

            if (pageIndex - 1 > 1)
            {
                sb.Append(pageSize > 1 ? $" * ({pageIndex} - 1)" : string.Empty);
            }

            sb.Append(" ROWS ").Append("FETCH NEXT ").Append(pageSize).Append(" ROWS ONLY");

            return sb.ToString();
        }

        ///<inheritdoc/>
        protected virtual string RenderJoins(IEnumerable<IJoin> joins)
        {
            joins = joins as IJoin[] ?? joins.ToArray();

            StringBuilder sbJoins = new();
            if (joins.AtLeastOnce())
            {
                foreach (IJoin @join in joins)
                {
                    if (sbJoins.Length > 0)
                    {
                        sbJoins = sbJoins.Append(' ');
                    }

                    sbJoins = @join.JoinType switch
                    {
                        JoinType.CrossJoin => sbJoins.Append($"CROSS JOIN {RenderTablename(@join.Table, renderAlias: true)}"),
                        JoinType.LeftOuterJoin => sbJoins.Append($"LEFT OUTER JOIN {RenderTablename(@join.Table, renderAlias: true)} ON {RenderWhere(@join.On)}"),
                        JoinType.RightOuterJoin => sbJoins.AppendFormat($"RIGHT OUTER JOIN {RenderTablename(@join.Table, renderAlias: true)} ON {RenderWhere(@join.On)}"),
                        JoinType.InnerJoin => sbJoins.AppendFormat($"INNER JOIN {RenderTablename(@join.Table, renderAlias: true)} ON {RenderWhere(@join.On)}"),
                        _ => throw new ArgumentOutOfRangeException(nameof(@join), join?.JoinType, "Unexpected join type")
                    };
                }
            }
            return sbJoins.ToString();
        }

        ///<inheritdoc/>
        protected virtual string RenderTables(IReadOnlyList<ITable> tables, int blockLevel = 0)
        {
            QueryWriter sbTables = new(blockLevel, Settings.PrettyPrint);
            for (int i = 0; i < tables.Count; i++)
            {
                ITable item = tables[i];
                if (sbTables.Length != 0)
                {
                    sbTables.WriteText(",");
                }

                if (item is Table table)
                {
                    if (string.IsNullOrWhiteSpace(table.Alias))
                    {
                        sbTables.WriteText(EscapeName(table.Name));
                    }
                    else
                    {
                        sbTables.WriteText(RenderTableNameWithAlias(EscapeName(table.Name), EscapeName(table.Alias)));
                    }
                }
                else if (item is SelectQuery selectTable)
                {
                    if (string.IsNullOrWhiteSpace(selectTable.Alias))
                    {
                        sbTables.StartBlock(LeftParenthesis);
                        sbTables.WriteText($"{Render(selectTable)}");
                        sbTables.EndBlock(RightParenthesis);
                    }
                    else
                    {
                        sbTables.WriteText($"({Render(selectTable, sbTables.BlockLevel)}) {EscapeName(selectTable.Alias)}");
                    }
                }
            }

            return sbTables.Value;
        }

        /// <summary>
        /// Renders <paramref name="columns"/> as <see langword="string"/>.
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="blockLevel"></param>
        /// <returns></returns>
        protected virtual string RenderColumns(IEnumerable<IColumn> columns, int blockLevel = 0)
        {
            columns = columns as IColumn[] ?? columns.ToArray();

            StringBuilder sbFields = new(columns.Count() * 25);

            foreach (IColumn column in columns)
            {
                if (sbFields.Length > 0)
                {
                    sbFields = sbFields.Append(", ");
                }
                if (column is CasesColumn cc)
                {
                    sbFields.Append(RenderCasesColumn(cc, renderAlias: false));

                    if (!string.IsNullOrWhiteSpace(cc.Alias))
                    {
                        sbFields.Append($" AS {EscapeName(cc.Alias)}");
                    }
                }
                else
                {
                    sbFields.Append(RenderColumn(column, renderAlias: true));
                }
            }
            QueryWriter writer = new(blockLevel, Settings.PrettyPrint);

            writer.WriteText(sbFields.ToString());

            return writer.Value;
        }

        ///<inheritdoc/>
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
                    StringValues variables => $"{RenderColumn(clause.Column, false)} IN ({string.Join(", ", variables.Select(x => RenderColumn(x.Literal(), false)))})",
                    SelectQuery select => $"{RenderColumn(clause.Column, false)} IN ({Render(select)})",
                    _ => throw new NotSupportedException($"Unsupported '{clause?.Constraint?.GetType()}' constraint type for {nameof(ClauseOperator)}.{nameof(ClauseOperator.In)} operator."),
                },
                ClauseOperator.NotIn => clause.Constraint switch
                {
                    VariableValues variables => $"{RenderColumn(clause.Column, false)} NOT IN ({string.Join(", ", variables.Select(x => RenderVariable(x, false)))})",
                    StringValues variables => $"{RenderColumn(clause.Column, false)} NOT IN ({string.Join(", ", variables.Select(x => RenderColumn(x.Literal(), false)))})",
                    SelectQuery select => $"{RenderColumn(clause.Column, false)} NOT IN ({Render(select)})",
                    _ => throw new NotSupportedException($"Unsupported '{clause?.Constraint?.GetType()}' constraint type for {nameof(ClauseOperator)}.{nameof(ClauseOperator.NotIn)} operator."),
                },
                _ => throw new NotSupportedException($"Unsupported '{clause.Operator}' clause operator"),
            };

        ///<inheritdoc/>
        protected virtual string RenderWhere(IWhereClause clause, int blockLevel = 0)
        {
            QueryWriter sbWhere = new QueryWriter(blockLevel, Settings.PrettyPrint);

            switch (clause)
            {
                case WhereClause whereClause:
                    sbWhere.WriteText(RenderClause(whereClause));
                    break;
                case CompositeWhereClause compositeClause:
                    switch (compositeClause.Logic)
                    {
                        case ClauseLogic.And:
                            foreach (IWhereClause innerClause in compositeClause.Clauses)
                            {
                                if (sbWhere.Length > 0)
                                {
                                    sbWhere.WriteText("AND");
                                }
                                sbWhere.WriteText(RenderWhere(innerClause, sbWhere.BlockLevel));
                            }

                            break;
                        case ClauseLogic.Or:
                            foreach (IWhereClause innerClause in compositeClause.Clauses)
                            {
                                if (sbWhere.Length > 0)
                                {
                                    sbWhere.WriteText("OR");
                                }

                                sbWhere.WriteText(RenderWhere(innerClause, sbWhere.BlockLevel));
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(compositeClause.Logic), compositeClause.Logic, "Unknown logic");
                    }

                    break;
            }

            return $"({sbWhere.Value})";
        }
        
        /// <summary>
        /// Renders the specified <paramref name="clause"/>.
        /// </summary>
        /// <param name="clause"></param>
        /// <returns>printable string</returns>
        /// <exception cref="ArgumentOutOfRangeException">the <paramref name="clause"/> is not supported by the renderer.</exception>
        protected virtual string RenderHaving(IHavingClause clause)
        {
            StringBuilder sbHaving = new();

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

        ///<inheritdoc/>
        protected virtual string RenderTablename(Table table, bool renderAlias)
            => !renderAlias || string.IsNullOrWhiteSpace(table.Alias)
                ? EscapeName(table.Name)
                : $"{EscapeName(table.Name)} {EscapeName(table.Alias)}";

        /// <summary>
        /// Renders the specified <paramref name="column"/>.
        /// </summary>
        /// <param name="column">The column to render.</param>
        /// <param name="renderAlias">hint which, if set to <see langword="true"/>, instructs to render the alias associated with the column alongside the column.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="column"/>'s type is not supported</exception>
        protected virtual string RenderColumn(IColumn column, bool renderAlias)
        {
            string columnString;
            switch (column)
            {
                case null:
                    columnString = "NULL";
                    break;
                default:
                    columnString = column.GetType().GetTypeInfo().GetCustomAttribute<FunctionAttribute>() is not null
                        ? RenderFunction(column, renderAlias)
                        : column switch
                        {
                            FieldColumn fieldColumn => !renderAlias || string.IsNullOrWhiteSpace(fieldColumn.Alias)
                                ? EscapeName(Settings.FieldnameCasingStrategy.Handle(fieldColumn.Name))
                                : RenderColumnNameWithAlias(EscapeName(Settings.FieldnameCasingStrategy.Handle(fieldColumn.Name)), EscapeName(fieldColumn.Alias)),
                            Literal literalColumn => RenderLiteralColumn(literalColumn, renderAlias),
                            SelectColumn selectColumn => RenderInlineSelect(selectColumn, renderAlias),
                            UniqueIdentifierValue _ => RenderUUIDValue(),
                            Variable variable => RenderVariable(variable, renderAlias),
                            SelectQuery select => RenderInlineSelect(select, renderAlias),
                            CasesColumn casesColumn => RenderCasesColumn(casesColumn, renderAlias),
                            _ => throw new ArgumentOutOfRangeException(nameof(column), column, $"Unexpected {column?.GetType()} rendering as column")
                        };

                    break;
            }

            return columnString;
        }

        /// <summary>
        /// Renders <see cref="CasesColumn"/>.
        /// </summary>
        /// <param name="caseColumn">The column to render.</param>
        /// <param name="renderAlias"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="caseColumn"/> is <see langword="null"/>.</exception>
        protected virtual string RenderCasesColumn(CasesColumn caseColumn, bool renderAlias)
        {
            StringBuilder sb = new();

            foreach (WhenExpression when in caseColumn.Cases)
            {
                if (sb.Length > 0)
                {
                    sb.Append(' ');
                }
                sb.Append("WHEN ")
                    .Append(RenderWhere(when.Criterion))
                    .Append(" THEN ")
                    .Append(RenderColumn(when.ThenValue, false));
            }

            if (caseColumn.Default is not null)
            {
                sb.Append(" ELSE ").Append(RenderColumn(caseColumn.Default, false));
            }

            return $"CASE {sb} END";
        }

        ///<inheritdoc/>
        protected virtual string RenderVariable(Variable variable, bool renderAlias) => throw new NotSupportedException();

        ///<inheritdoc/>
        protected virtual string RenderUUIDValue() => "NEWID()";

        ///<inheritdoc/>
        protected virtual string RenderFunction(IColumn column, bool renderAlias)
            => column switch
            {
                AggregateFunction aggregateColumn => RenderAggregateColumn(aggregateColumn, renderAlias),
                ConcatFunction concatColumn => RenderConcatColumn(concatColumn, renderAlias),
                NullFunction nullColumn => RenderNullColumn(nullColumn, renderAlias),
                LengthFunction lengthColumn => RenderLengthColumn(lengthColumn, renderAlias),
                SubstringFunction substringColumn => RenderSubstringColumn(substringColumn, renderAlias),
                UpperFunction upperColumn => RenderUpperColumn(upperColumn, renderAlias),
                SubstractFunction substractColumn => RenderSubstractColumn(substractColumn, renderAlias),
                _ => throw new ArgumentOutOfRangeException(nameof(column), column, "Unexpected function type"),
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

        /// <summary>
        /// Renders <see cref="LengthFunction"/>
        /// </summary>
        /// <param name="lengthColumn">The length function</param>
        /// <param name="renderAlias">hint to render the alias or not.</param>
        /// <returns></returns>
        protected virtual string RenderLengthColumn(LengthFunction lengthColumn, bool renderAlias)
        {
            string sbLengthColumn = $"{LengthFunctionName}({RenderColumn(lengthColumn.Column, false)})";

            return renderAlias && !string.IsNullOrWhiteSpace(lengthColumn.Alias)
                ? RenderColumnNameWithAlias(sbLengthColumn, EscapeName(lengthColumn.Alias))
                : sbLengthColumn;
        }


        /// <summary>
        /// Renders <see cref="SubstractFunction"/>
        /// </summary>
        /// <param name="substringColumn"></param>
        /// <param name="renderAlias">hint to render the alias or not.</param>
        /// <returns></returns>
        protected virtual string RenderSubstringColumn(SubstringFunction substringColumn, bool renderAlias)
        {
            string sbLengthColumn = $"{SubstringFunctionName}({RenderColumn(substringColumn.Column, false)}, {substringColumn.Start}{(substringColumn.Length.HasValue ? $", {substringColumn.Length.Value}" : "")})";

            string queryString = renderAlias && !string.IsNullOrWhiteSpace(substringColumn.Alias)
                ? RenderColumnNameWithAlias(sbLengthColumn, EscapeName(substringColumn.Alias))
                : sbLengthColumn;

            return queryString;
        }

        /// <summary>
        /// Renders <see cref="UpperFunction"/>
        /// </summary>
        /// <param name="upperColumn"></param>
        /// <param name="renderAlias">hint to render the alias or not.</param>
        /// <returns></returns>
        protected virtual string RenderUpperColumn(UpperFunction upperColumn, bool renderAlias)
        {
            string sbLengthColumn = $"{UpperFunctionName}({RenderColumn(upperColumn.Column, false)})";

            return renderAlias && !string.IsNullOrWhiteSpace(upperColumn.Alias)
                ? RenderColumnNameWithAlias(sbLengthColumn, EscapeName(upperColumn.Alias))
                : sbLengthColumn;
        }

        /// <summary>
        /// Renders <see cref="ConcatFunction"/>
        /// </summary>
        /// <param name="concatColumn"></param>
        /// <param name="renderAlias">hint to render the alias or not.</param>
        /// <returns></returns>
        protected virtual string RenderConcatColumn(ConcatFunction concatColumn, bool renderAlias)
        {
            StringBuilder sbConcat = new();
            const string space = " ";
            foreach (IColumn column in concatColumn.Columns)
            {
                if (sbConcat.Length > 0)
                {
                    sbConcat = sbConcat.Append(space).Append(ConcatOperator).Append(space);
                }
                sbConcat = sbConcat.Append(RenderColumn(column, renderAlias: false));
            }

            return renderAlias && !string.IsNullOrWhiteSpace(concatColumn.Alias)
                ? RenderColumnNameWithAlias(sbConcat.ToString(), EscapeName(concatColumn.Alias))
                : sbConcat.ToString();
        }

        /// <summary>
        /// Renders <see cref="NullFunction"/>
        /// </summary>
        /// <param name="nullColumn"></param>
        /// <param name="renderAlias"></param>
        /// <returns></returns>
        protected virtual string RenderNullColumn(NullFunction nullColumn, bool renderAlias)
        {
            string sbNullColumn = $"ISNULL({RenderColumn(nullColumn.Column, false)}, {RenderColumn(nullColumn.DefaultValue, false)})";

            return renderAlias && !string.IsNullOrWhiteSpace(nullColumn.Alias)
                ? RenderColumnNameWithAlias(sbNullColumn, EscapeName(nullColumn.Alias))
                : sbNullColumn;
        }

        /// <summary>
        /// Renders <see cref="SubstractFunction"/>
        /// </summary>
        /// <param name="substractColumn"></param>
        /// <param name="renderAlias">hint to render the alias or not.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="substractColumn"/> is <see langword="null"/>.</exception>
        protected virtual string RenderSubstractColumn(SubstractFunction substractColumn, bool renderAlias)
            => $"{RenderColumn(substractColumn.Left, false)} - {RenderColumn(substractColumn.Right, false)}";

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

        ///<inheritdoc/>
        protected virtual string RenderInlineSelect(SelectColumn inlineSelectQuery, bool renderAlias)
            => !renderAlias || string.IsNullOrWhiteSpace(inlineSelectQuery.Alias)
                ? $"({Render(inlineSelectQuery.SelectQuery)})"
                : RenderColumnNameWithAlias($"({Render(inlineSelectQuery.SelectQuery)})", EscapeName(inlineSelectQuery.Alias));

        /// <summary>
        /// Renders the specified <paramref name="tableName"/>
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="alias">hint to render the alias or not.</param>
        /// <returns></returns>
        protected virtual string RenderTableNameWithAlias(string tableName, string alias) => $"{tableName} {alias}";

        /// <summary>
        /// Renders the specified <paramref name="columnName"/>.
        /// </summary>
        /// <param name="columnName">The column name to render.</param>
        /// <param name="alias">hint to render the alias or not.</param>
        /// <returns></returns>
        protected virtual string RenderColumnNameWithAlias(string columnName, string alias) => $"{columnName} AS {alias}";


        /// <summary>
        /// Renders the specified <see cref="AggregateFunction"/>
        /// </summary>
        /// <param name="ac">The aggregate function to render</param>
        /// <param name="renderAlias">hint use to render or not the alias associated with <paramref name="ac"/>.</param>
        /// <returns>string representation of the specified <paramref name="ac"/>.</returns>
        protected virtual string RenderAggregateColumn(AggregateFunction ac, bool renderAlias)
            => ac.Type switch
            {
                AggregateType.Min => !renderAlias || string.IsNullOrWhiteSpace(ac.Alias)
                        ? $"MIN({RenderColumn(ac.Column, renderAlias: false)})"
                        : RenderColumnNameWithAlias($"MIN({RenderColumn(ac.Column, renderAlias: false)})", EscapeName(ac.Alias)),
                AggregateType.Max => !renderAlias || string.IsNullOrWhiteSpace(ac.Alias)
                        ? $"MAX({RenderColumn(ac.Column, renderAlias: false)})"
                        : RenderColumnNameWithAlias($"MAX({RenderColumn(ac.Column, renderAlias: false)})", EscapeName(ac.Alias)),
                AggregateType.Average => !renderAlias || string.IsNullOrWhiteSpace(ac.Alias)
                        ? $"AVG({RenderColumn(ac.Column, renderAlias: false)})"
                        : RenderColumnNameWithAlias($"AVG({RenderColumn(ac.Column, renderAlias: false)})", EscapeName(ac.Alias)),
                AggregateType.Count => !renderAlias || string.IsNullOrWhiteSpace(ac.Alias)
                        ? $"COUNT({RenderColumn(ac.Column, renderAlias: false)})"
                        : RenderColumnNameWithAlias($"COUNT({RenderColumn(ac.Column, renderAlias: false)})", EscapeName(ac.Alias)),
                _ => throw new ArgumentOutOfRangeException(nameof(ac), ac, "Unexpected aggregate function type"),
            };

        /// <summary>
        /// Renders a <see cref="Literal"/>
        /// </summary>
        /// <param name="lc">The literal to render</param>
        /// <param name="renderAlias"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
                BooleanColumn bc => renderAlias && !string.IsNullOrWhiteSpace(bc.Alias)
                                    ? $"{(true.Equals(bc.Value) ? "1" : "0")} AS {EscapeName(bc.Alias)}"
                                    : true.Equals(bc.Value) ? "1" : "0",
                _ => renderAlias && !string.IsNullOrWhiteSpace(lc.Alias)
                            ? $"{value} AS {EscapeName(lc.Alias)}"
                            : value.ToString()
            };
        }

        /// <summary>
        /// Escape a <see cref="string"/> value so that it can be safely used in a string query
        /// </summary>
        /// <param name="unescapedString">the value to escape.</param>
        /// <returns>a safe <see cref="string"/></returns>
        protected virtual string EscapeString(string unescapedString) => unescapedString?.Replace("'", "''");

        /// <summary>
        /// Renders <see cref="UpdateQuery"/>
        /// </summary>
        /// <param name="updateQuery"></param>
        /// <returns></returns>
        protected virtual string Render(UpdateQuery updateQuery)
        {
            StringBuilder queryStringBuilder = new();

            if (updateQuery is not null)
            {
                StringBuilder sbFieldsToUpdate = new();
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

                if (updateQuery.Criteria is not null)
                {
                    queryStringBuilder = queryStringBuilder
                        .Append(' ')
                        .Append(Settings.PrettyPrint ? Environment.NewLine : string.Empty)
                        .Append($"WHERE {RenderWhere(updateQuery.Criteria)}");
                }
            }

            return queryStringBuilder.ToString();
        }

        /// <summary>
        /// Renders <see cref="CreateViewQuery"/>
        /// </summary>
        /// <param name="query"></param>
        /// <returns>string representation of the specified <paramref name="query"/>.</returns>
        protected virtual string Render(CreateViewQuery query)
        {
            StringBuilder sb = new();

            sb = sb.Append($"CREATE VIEW {RenderTables([query.ViewName.Table()])} ")
                .Append(Settings.PrettyPrint ? Environment.NewLine : string.Empty)
                .AppendFormat("AS ")
                .Append(Settings.PrettyPrint ? Environment.NewLine : string.Empty)
                .Append(Render(query.SelectQuery));

            return sb.ToString();
        }

        /// <summary>
        /// Renders <see cref="TruncateQuery"/>.
        /// </summary>
        /// <param name="query">The query to render</param>
        /// <returns>representation of the specified <paramref name="query"/>.</returns>
        protected virtual string Render(TruncateQuery query)
        {
            string sbQuery = string.Empty;
            if (query is not null)
            {
                sbQuery = $"TRUNCATE TABLE {BeginEscapeWordString}{query.Name}{EndEscapeWordString}";
            }

            return sbQuery;
        }

        /// <summary>
        /// Renders <see cref="DeleteQuery"/>.
        /// </summary>
        /// <param name="deleteQuery">The query to render</param>
        /// <returns></returns>
        protected virtual string Render(DeleteQuery deleteQuery)
        {
            if (deleteQuery is null)
            {
                return string.Empty;
            }

            StringBuilder sbQuery = new($"DELETE FROM {EscapeName(deleteQuery.Table)}");

            if (deleteQuery.Criteria is not null)
            {
                sbQuery = sbQuery
                    .Append($" {( Settings.PrettyPrint ? Environment.NewLine : string.Empty )}WHERE {RenderWhere(deleteQuery.Criteria)}");
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
            StringBuilder sbResult = new();

            IReadOnlyList<IQuery> statements = query.Statements ?? [];

            foreach (IQuery statement in statements)
            {
                sbResult
                    .Append(Render(statement))
                    .Append(BatchStatementSeparator);
            }

            return sbResult.ToString();
        }

        /// <summary>
        /// Separator for multiple statements in a batch of queries.
        /// </summary>
        public virtual string BatchStatementSeparator => ";";


        public virtual CompiledQuery Compile(IQuery query)
        {
            CollectVariableVisitor visitor = new();
            CompiledQuery compiledQuery;
            switch (query)
            {
                case SelectQuery selectQuery:
                    visitor.Visit(selectQuery);
                    compiledQuery = new CompiledQuery(Render(selectQuery), visitor.Variables);
                    break;
                default:
                    compiledQuery = new CompiledQuery(Render(query), []);
                    break;
            }

            return compiledQuery;
        }
    }
}