using System;
using System.Collections.Generic;
using System.Linq;
using Queries.Core.Builders.Fluent;
using Queries.Core.Extensions;
using Queries.Core.Parts;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Joins;
using Queries.Core.Parts.Sorting;

namespace Queries.Core.Builders
{
    public class SelectQuery : SelectQueryBase, ISelectQuery<SelectQuery>, IFromQuery<SelectQuery>, IWhereQuery<SelectQuery>, IJoinQuery<SelectQuery>, ISortQuery<SelectQuery>, IInsertable
    {
        private int? _limit;

        /// <summary>
        /// Defines the max number of records to retrieve
        /// </summary>
        public int? NbRows => _limit;
       

        public IList<ITable> Tables { get; }
        public IList<IUnionQuery<SelectQuery>> Unions { get; set; }

        
        public SelectQuery(params IColumn[] columns)
        {
            Columns = columns;
            Tables = new List<ITable>();
            Unions = new List<IUnionQuery<SelectQuery>>();
        }

        internal SelectQuery(params string[] columnNames) : this(columnNames.Select(colName => colName.Field()).Cast<IColumn>().ToArray())
        {
        }

        public ISelectQuery<SelectQuery> Limit(int limit)
        {
            _limit = limit;
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

        public IWhereQuery<SelectQuery> Where(IColumn column, ClauseOperator @operator, ColumnBase constraint) 
            => Where(new WhereClause(column, @operator, constraint));




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



        ISortQuery<SelectQuery> IWhereQuery<SelectQuery>.OrderBy(params ISort[] sorts)
        {
            foreach (ISort sort in sorts)
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

        public ISortQuery<SelectQuery> OrderBy(params ISort[] sorts)
        {
            foreach (ISort sort in sorts)
            {
                Sorts.Add(sort);
            }
            return this;
        }


        public ISortQuery<SelectQuery> Having(IHavingClause clause)
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
    }

}