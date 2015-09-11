using System;
using Queries.Parts;
using Queries.Parts.Clauses;
using Queries.Parts.Columns;
using Queries.Parts.Joins;

namespace Queries.Builders.Fluent
{
    public class Select : ISelectQuery<SelectQuery>, IFromQuery<SelectQuery>, IWhereQuery<SelectQuery>, IJoinQuery<SelectQuery>
    {
        private readonly SelectQuery _query;

        #region Constructors

        public Select()
        {
            _query = new SelectQuery();
        }


        

        public Select(params IColumn[] columns) : this()
        {
            foreach (IColumn column in columns)
            {
                _query.Select.Add(column);
            }
        }


        public Select(params string[] columnNames) : this()
        {
            foreach (string column in columnNames)
            {
                _query.Select.Add(FieldColumn.From(column));
            }
        }

        
        #endregion

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

        public IFromQuery<SelectQuery> From(SelectQuery @select)
        {
            throw new NotImplementedException();
        }

        public IFromQuery<SelectQuery> From(string tableName)
        {
            _query.From.Add(new Table(){Name = tableName});

            return this;
        }

        public IWhereQuery<SelectQuery> Where(IWhereClause clause)
        {
            _query.Where = clause;

            return this;
        }

        public IJoinQuery<SelectQuery> InnerJoin(Table table, IWhereClause clause)
        {
            _query.Joins.Add(new InnerJoin(table, clause));

            return this;
        }

        public IJoinQuery<SelectQuery> LeftOuterJoin(Table table, IWhereClause clause)
        {
            _query.Joins.Add(new LeftOuterJoin(table, clause));

            return this;
        }

        public IJoinQuery<SelectQuery> RightOuterJoin(Table table, IWhereClause clause)
        {
            _query.Joins.Add(new RightOuterJoin(table, clause));

            return this;
        }


        

        public SelectQuery Build()
        {
            return _query;
        }

        public IWhereQuery<SelectQuery> InnerJoin(IWhereClause clause)
        {
            throw new NotImplementedException();
        }
    }
}