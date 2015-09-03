using System;
using Queries.Parts;
using Queries.Parts.Columns;

namespace Queries.Builders.Fluent
{
    public class SelectQueryBuilder : ISqlSelect, ISqlFrom, ISqlWhere
    {
        private readonly SelectQuery _query;


        public SelectQueryBuilder()
        {
            _query = new SelectQuery();
        }


        public ISqlFrom Select(IColumn col, params IColumn[] columns)
        {

            if (col == null)
            {
                throw new ArgumentNullException("col");
            }



            foreach (IColumn column in columns)
            {
                _query.Select.Add(column);
            }


            return this;
        }

        public ISqlWhere From(TableTerm table, params TableTerm[] tables)
        {
            _query.From.Add(table);

            foreach (TableTerm tableTerm in tables)
            {
                _query.From.Add(tableTerm);
            }

            return this;
        }

        public ISqlWhere From(SelectQuery @select)
        {
            throw new NotSupportedException();
        }

        public ISqlWhere Where(IClause clause)
        {
            _query.Where = clause;

            return this;
        }


        public ISqlOrder OrderBy(IColumn column, params IColumn[] cols)
        {
            throw new NotImplementedException();
        }
    }
}