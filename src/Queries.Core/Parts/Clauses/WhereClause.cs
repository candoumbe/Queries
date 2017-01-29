using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Clauses
{
    public class WhereClause : IWhereClause, IClause<IColumn>
    {
        public IColumn Column { get; }
        public ClauseOperator Operator{ get; }
        public ColumnBase Constraint { get; }

        public WhereClause(IColumn column, ClauseOperator @operator, ColumnBase constraint = null)
        {
            Column = column;
            Operator = @operator;
            Constraint = constraint;
        }
    }
}
