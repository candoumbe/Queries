using System;
using Queries.Parts;
using Queries.Parts.Clauses;

namespace Queries.Builders.Fluent
{
    public class Delete : IDeleteQuery<DeleteQuery>
    {
        private readonly DeleteQuery _query;

        internal Delete(Table table)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }
            _query = new DeleteQuery(){Table = table};

        }


        public IBuildableQuery<DeleteQuery> Where(IWhereClause clause)
        {
            if (clause == null)
            {
                throw new ArgumentNullException(nameof(clause));
            }

            _query.Where = clause;

            return this;
        }

        public DeleteQuery Build()
        {
            return _query;
        }
    }
}
