﻿using System;
using System.Text;
using Queries.Core.Renderers;
using Queries.Core.Parts.Functions;
using Queries.Core.Parts.Columns;
using Queries.Renderers.Postgres.Parts.Columns;
using System.Linq;
using Queries.Core.Parts.Clauses;
using Queries.Renderers.Postgres.Builders;
using Queries.Core;
using Queries.Core.Exceptions;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Renderers.Postgres
{
    public class PostgresqlRenderer : QueryRendererBase
    {
        /// <summary>
        /// Creates a new renderer for Postgres. <br/>
        /// 
        /// <para>
        ///     
        /// </para>
        /// </summary>
        /// <param name="settings">Defines how to render <see cref="IQuery"/></param>
        public PostgresqlRenderer(PostgresRendererSettings settings = null)
            : base(settings ?? new PostgresRendererSettings { DateFormatString = "YYYY-mm-DD", PrettyPrint = true })
        { }

        protected override string RenderColumn(IColumn column, bool renderAlias)
            => column is JsonFieldColumn json
                           ? RenderJsonColumn(json, renderAlias)
                           : base.RenderColumn(column, renderAlias);

        protected virtual string RenderJsonColumn(JsonFieldColumn json, bool renderAlias)
        {
            string[] columnParts = json.Path.Split('.');
            string result;
            if (columnParts.Length == 1)
            {
                result = $"{RenderColumn(json.Column, renderAlias: false)} {(json.RenderAsString ? "->>" : "->")} '{columnParts.Single()}'";
            }
            else
            {
                string path = $"{string.Join(" -> ", columnParts.Take(columnParts.Length - 1).Select(EscapeName))} {(json.RenderAsString ? "->>" : "->")} '{ columnParts.Last() }'";
                result = $"{RenderColumn(json.Column, renderAlias: false)} -> {path}";
            }

            return $"{result}{(renderAlias && !string.IsNullOrWhiteSpace(json.Alias) ? $" AS {EscapeName(json.Alias)}" : string.Empty)}";
        }

        public override string Render(IQuery query)
            => query is ReturnQuery returnQuery
                ? returnQuery.Return.Match(
                    columnBase =>
                    {
                        return columnBase switch
                        {
                            FieldColumn field => $"RETURN {RenderColumn(field, renderAlias: false)}",
                            Literal literal => $"RETURN {Render(Select(literal)).Substring("SELECT ".Length)}",
                            null => "RETURN",
                            _ => throw new InvalidQueryException(),
                        };
                    },
                    select => $"RETURN {base.Render(select)}"
                )
                : base.Render(query);

        protected override string RenderWhere(IWhereClause clause)
        {
            string result = string.Empty;

            switch (clause)
            {
                case WhereClause where when where.Column is JsonFieldColumn json:
                    switch (where.Operator)
                    {
                        case ClauseOperator.EqualTo:
                            result = $"({RenderJsonColumn(new JsonFieldColumn(json.Column, json.Path, renderAsString: where.Constraint is StringColumn), renderAlias: false)} = {RenderColumn(where.Constraint, renderAlias: false)})";
                            break;
                        default:
                            break;
                    }
                    break;
                case WhereClause where when where.Constraint is JsonFieldColumn jsonConstraint:
                    switch (where.Operator)
                    {
                        case ClauseOperator.EqualTo:
                            result = $"({RenderColumn(where.Column, renderAlias: false)} = {RenderJsonColumn(new JsonFieldColumn(jsonConstraint.Column, jsonConstraint.Path, renderAsString: where.Column is StringColumn), renderAlias: false)})";
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    result = base.RenderWhere(clause);

                    break;
            }

            return result;
        }

        protected override string EndEscapeWordString => @"""";

        protected override string ConcatOperator => "||";

        protected override string RenderColumnnameWithAlias(string columnName, string alias) => $"{columnName} {alias}";

        protected override string RenderUUIDValue() => "uuid_generate_v4()";

        protected override string RenderNullColumn(NullFunction nullColumn, bool renderAlias)
        {
            StringBuilder sbNullColumn = new StringBuilder();

            sbNullColumn = sbNullColumn.Append("COALESCE(")
                .Append(RenderColumn(nullColumn.Column, false)).Append(", ").Append(RenderColumn(nullColumn.DefaultValue, false))
                .Append(")");

            return renderAlias && !string.IsNullOrWhiteSpace(nullColumn.Alias)
                ? RenderColumnnameWithAlias(sbNullColumn.ToString(), EscapeName(nullColumn.Alias))
                : sbNullColumn.ToString();
        }

        protected override string BeginEscapeWordString => @"""";

        protected override string RenderVariable(Variable variable, bool renderAlias) => $"@{variable.Name}";

        protected override string RenderSubstringColumn(SubstringFunction substringColumn, bool renderAlias) => $"SUBSTRING({RenderColumn(substringColumn.Column, false)} FROM {substringColumn.Start}{(substringColumn.Length.HasValue ? $" FOR {substringColumn.Length.Value}" : string.Empty)})";

        protected override string RenderPagination(int pageIndex, int pageSize)
        {
            StringBuilder sb = new StringBuilder()
                .Append("LIMIT ").Append(pageSize);

            if (pageIndex >= 1)
            {
                sb.Append(" OFFSET ").Append(pageSize);
            }

            if ((pageIndex - 1) >= 2)
            {
                sb.Append(" * ").Append(pageIndex - 1);
            }

            return sb.ToString();
        }
    }
}
