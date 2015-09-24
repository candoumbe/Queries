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
    public class SelectQuery : SelectQueryBase, ISelectQuery<SelectQuery>, IFromQuery<SelectQuery>, IWhereQuery<SelectQuery>, IJoinQuery<SelectQuery>, ISortQuery<SelectQuery>, IHavingQuery<SelectQuery>, IUnionQuery<SelectQuery>
    {
        private int? _limit;

        public int? NbRows => _limit;
       

        public IList<ITable> Tables { get; }
        public IList<IUnionQuery<SelectQuery>> Unions { get; set; }

        
        internal SelectQuery(params IColumn[] columns)
        {
            Columns = columns;
            Tables = new List<ITable>();
            Unions = new List<IUnionQuery<SelectQuery>>();
        }


        

        internal SelectQuery(params string[] columnNames)
        {
            Columns = columnNames.Select(colName => colName.Field()).Cast<IColumn>().ToList();
            Tables = new List<ITable>();
            Unions = new List<IUnionQuery<SelectQuery>>();

        }

        public IFromQuery<SelectQuery> Limit(int limit)
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

        public IFromQuery<SelectQuery> From(SelectTable @select)
        {
            if (select == null)
            {
                throw new ArgumentNullException(nameof(@select), "select cannot be null");
            }
            Tables.Add(@select);

            return this;
        }

        

        public IWhereQuery<SelectQuery> Where(IWhereClause clause)
        {
            if (clause == null)
            {
                throw new ArgumentNullException(nameof(clause), "clause cannot be null");
            }

            WhereCriteria = clause;

            return this;
        }

        public IJoinQuery<SelectQuery> InnerJoin(Table table, IWhereClause clause)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table), "table cannot be null");
            }
            
            if (clause == null)
            {
                throw new ArgumentNullException(nameof(clause), "clause cannot be null");
            }

            Joins.Add(new InnerJoin(table, clause));

            return this;
        }

        public IJoinQuery<SelectQuery> LeftOuterJoin(Table table, IWhereClause clause)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table), "table cannot be null");
            }

            if (clause == null)
            {
                throw new ArgumentNullException(nameof(clause), "clause cannot be null");
            }

            Joins.Add(new LeftOuterJoin(table, clause));

            return this;
        }

        public IJoinQuery<SelectQuery> RightOuterJoin(Table table, IWhereClause clause)
        {

            if (table == null)
            {
                throw new ArgumentNullException(nameof(table), "table cannot be null");
            }

            if (clause == null)
            {
                throw new ArgumentNullException(nameof(clause), "clause cannot be null");
            }

            Joins.Add(new RightOuterJoin(table, clause));

            return this;
        }

        public SelectQuery Build()
        {
            return this;
        }

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
    }

}