using Queries.Core.Builders.Fluent;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Builders
{
    public class DeleteQuery : IDataManipulationQuery, IBuildableQuery<DeleteQuery>
    {
        public string Table { get; private set; }

        public IWhereClause Criteria { get; private set; }


        internal DeleteQuery(string tableName)
        {
            Table = tableName;
        }


        public IBuildableQuery<DeleteQuery> Where(IWhereClause clause)
        {
            Criteria = clause;

            return this;
        }


        public IBuildableQuery<DeleteQuery> Where(FieldColumn column, ClauseOperator @operator, ColumnBase columnBase)
        {
            return Where(new WhereClause(column, @operator, columnBase));
        } 

        public DeleteQuery Build() => this;

    }
}