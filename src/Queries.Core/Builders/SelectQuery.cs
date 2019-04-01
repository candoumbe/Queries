using Newtonsoft.Json;
using Queries.Core.Builders.Fluent;
using Queries.Core.Parts;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Joins;
using Queries.Core.Parts.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Queries.Core.Builders
{
    [JsonObject(ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
    public class SelectQuery : SelectQueryBase, ISelectQuery<SelectQuery>, IFromQuery<SelectQuery>, IWhereQuery<SelectQuery>, IJoinQuery<SelectQuery>, IOrderQuery<SelectQuery>, IEquatable<SelectQuery>, IColumn
    {
        /// <summary>
        /// Defines the max number of records to retrieve
        /// </summary>
        public int? NbRows { get; private set; }

        public IList<ITable> Tables { get; }
        public IList<IUnionQuery<SelectQuery>> Unions { get; set; }

        /// <summary>
        /// Builds a new <see cref="SelectQuery"/> instance.
        /// </summary>
        /// <param name="columns">columns of the query</param>
        /// <remarks>
        /// This constructor filters out <c>null</c> value from <paramref name="columns"/>.
        /// </remarks>
        public SelectQuery(params IColumn[] columns)
        {
            if (!columns.Any(x => x != null))
            {
                throw new ArgumentOutOfRangeException(nameof(columns), "at least one column must be provided");
            }

            Columns = columns;
            Tables = new List<ITable>();
            Unions = new List<IUnionQuery<SelectQuery>>();
        }

        internal SelectQuery(params string[] columnNames) : this(columnNames.Select(colName => colName.Field()).Cast<IColumn>().ToArray())
        {
        }

        public ISelectQuery<SelectQuery> Limit(int limit)
        {
            NbRows = limit;
            return this;
        }

        public IFromQuery<SelectQuery> From(params ITable[] tables)
        {
            foreach (ITable tableTerm in tables)
            {
                Tables.Add(tableTerm);
            }

            return this;
        }

        public IFromQuery<SelectQuery> From(params string[] tables)
        {
            foreach (string tablename in tables)
            {
                Tables.Add(tablename.Table());
            }

            return this;
        }

        public IWhereQuery<SelectQuery> Where(IWhereClause clause)
        {
            WhereCriteria = clause ?? throw new ArgumentNullException(nameof(clause), $"{clause} cannot be null");

            return this;
        }

        public IWhereQuery<SelectQuery> Where(IColumn column, ClauseOperator @operator, IColumn constraint)
            => Where(new WhereClause(column, @operator, constraint));

        public IWhereQuery<SelectQuery> Where(IColumn column, ClauseOperator @operator, string constraint)
            => Where(column, @operator, constraint?.Literal());

        public IJoinQuery<SelectQuery> InnerJoin(Table table, IWhereClause clause)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table), $"{nameof(table)} cannot be null");
            }

            if (clause == null)
            {
                throw new ArgumentNullException(nameof(clause), $"{nameof(clause)} cannot be null");
            }

            Joins.Add(new InnerJoin(table, clause));

            return this;
        }

        public IJoinQuery<SelectQuery> LeftOuterJoin(Table table, IWhereClause clause)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table), $"{nameof(table)} cannot be null");
            }

            if (clause == null)
            {
                throw new ArgumentNullException(nameof(clause), $"{nameof(clause)} cannot be null");
            }

            Joins.Add(new LeftOuterJoin(table, clause));

            return this;
        }

        public IJoinQuery<SelectQuery> RightOuterJoin(Table table, IWhereClause clause)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table), $"{nameof(table)} cannot be null");
            }

            if (clause == null)
            {
                throw new ArgumentNullException(nameof(clause), $"{nameof(clause)} cannot be null");
            }

            Joins.Add(new RightOuterJoin(table, clause));

            return this;
        }

        public SelectQuery Build() => this;

        IOrderQuery<SelectQuery> IWhereQuery<SelectQuery>.OrderBy(IOrder sort, params IOrder[] sorts)
        {
            Sorts.Add(sort);

            foreach (IOrder items in sorts.Where(s => s != default))
            {
                Sorts.Add(sort);
            }
            return this;
        }

        public IUnionQuery<SelectQuery> Union(IUnionQuery<SelectQuery> select)
        {
            Unions.Add(select);
            return this;
        }

        public IOrderQuery<SelectQuery> OrderBy(params IOrder[] sorts)
        {
            foreach (IOrder sort in sorts)
            {
                Sorts.Add(sort);
            }
            return this;
        }

        public IOrderQuery<SelectQuery> Having(IHavingClause clause)
        {
            HavingCriteria = clause;
            return this;
        }

        public string Alias { get; private set; }

        public ITable As(string alias)
        {
            Alias = alias;
            return this;
        }

        public override bool Equals(object obj) => Equals(obj as SelectQuery);

        public bool Equals(SelectQuery other)
        {
            bool equals = false;

            if (other != null && other.NbRows == NbRows && other.Alias == Alias)
            {
                equals = Columns.SequenceEqual(other.Columns)
                    && ((WhereCriteria == null && other.WhereCriteria == null) || WhereCriteria.Equals(other.WhereCriteria))
                    && Tables.SequenceEqual(other.Tables)
                    && Unions.SequenceEqual(other.Unions);
            }

            return equals;
        }

        public override int GetHashCode() => (Alias, Columns, HavingCriteria, Joins, NbRows, Sorts, Tables, Unions, WhereCriteria).GetHashCode();

        /// <summary>
        /// Performs a deep copy of the current instance.
        /// </summary>
        /// <returns>a <see cref="SelectQuery"/> instance that is a deep copy of the current instance.</returns>
        public SelectQuery Clone()
        {
            SelectQuery query = new SelectQuery(Columns.Select(x => x.Clone()).ToArray())
            {
                Alias = Alias,
                Columns = Columns.Select(x => x.Clone()).ToList(),
                HavingCriteria = HavingCriteria?.Clone(),
                WhereCriteria = WhereCriteria?.Clone()
            };
            if (NbRows.HasValue)
            {
                query.Limit(NbRows.Value);
            }
            query.From(Tables.Select(t => t.Clone()).ToArray());

            foreach (IUnionQuery<SelectQuery> item in Unions)
            {
                query.Union(item.Build().Clone());
            }

            return query;
        }

        ITable ITable.Clone() => Clone();

        IColumn IColumn.Clone() => Clone();

        public override string ToString()
        {
            object obj = new
            {
                Alias,
                Columns = Columns?.AtLeastOnce() ?? false ? Columns : null,
                Having= HavingCriteria?.ToString(),
                Joins = Joins.AtLeastOnce() ? Joins : null,
                NbRows,
                Sorts = Sorts.AtLeastOnce() ? Sorts : null,
                Tables = Tables.AtLeastOnce() ? Tables : null,
                Unions = Unions.AtLeastOnce() ? Unions : null,
                Where = WhereCriteria?.ToString()
            };

            return $"[{nameof(SelectQuery)}:{obj.Stringify()}]";
        }
    }
}