using System;
using Queries.Extensions;
using Queries.Parts;
using Queries.Parts.Clauses;
using Queries.Parts.Columns;
using Queries.Parts.Joins;
using Queries.Parts.Sorting;

namespace Queries.Builders.Fluent
{
    public class Select : ISelectQuery<SelectQuery>, IFromQuery<SelectQuery>, IWhereQuery<SelectQuery>, IJoinQuery<SelectQuery>, ISortQuery<SelectQuery>
    {
        private readonly SelectQuery _query;

        #region Constructors

        internal Select()
        {
            _query = new SelectQuery();
        }




        internal Select(params IColumn[] columns)
            : this()
        {
            foreach (IColumn column in columns)
            {
                _query.Select.Add(column);
            }
        }


        internal Select(params string[] columnNames)
            : this()
        {
            foreach (string column in columnNames)
            {
                _query.Select.Add(column.Field());
            }
        }

        
        #endregion

        public IFromQuery<SelectQuery> Limit(int limit)
        {
            _query.Limit = limit;
            return this;
        }

        public IFromQuery<SelectQuery> From(params ITable[] tables)
        {            
            foreach (ITable tableTerm in tables)
            {
                _query.From.Add(tableTerm);
            }

            return this;
        }

        public IFromQuery<SelectQuery> From(params string[] tables)
        {
            foreach (string tablename in tables)
            {
                _query.From.Add(new Table(){Name = tablename});
            }
            
            return this;
        }

        public IFromQuery<SelectQuery> From(SelectTable @select)
        {
            if (select == null)
            {
                throw new ArgumentNullException(nameof(@select), "select cannot be null");
            }
            _query.From.Add(@select);

            return this;
        }

        public IFromQuery<SelectQuery> From(string tableName)
        {
            _query.From.Add(new Table(){Name = tableName});

            return this;
        }

        public IWhereQuery<SelectQuery> Where(IWhereClause clause)
        {
            if (clause == null)
            {
                throw new ArgumentNullException(nameof(clause), "clause cannot be null");
            }

            _query.Where = clause;

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

            _query.Joins.Add(new InnerJoin(table, clause));

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

            _query.Joins.Add(new LeftOuterJoin(table, clause));

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

            _query.Joins.Add(new RightOuterJoin(table, clause));

            return this;
        }

        public SelectQuery Build()
        {
            return _query;
        }

        ISortQuery<SelectQuery> IWhereQuery<SelectQuery>.OrderBy(params ISort[] sorts)
        {
            foreach (ISort sort in sorts)
            {
                _query.OrderBy.Add(sort);
            }
            return this;
        }

        public ISortQuery<SelectQuery> OrderBy(params ISort[] sorts)
        {
            foreach (ISort sort in sorts)
            {
                _query.OrderBy.Add(sort);
            }
            return this;
        }

    }
}