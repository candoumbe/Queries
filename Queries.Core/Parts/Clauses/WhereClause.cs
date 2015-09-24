using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Clauses
{
    public class WhereClause : IWhereClause, IClause<IColumn>
    {
        public IColumn Column { get; }
        public ClauseOperator Operator{ get; }
        public ColumnBase Constraint { get; }

        public WhereClause(FieldColumn column, ClauseOperator @operator, ColumnBase constraint)
        {
            Column = column;
            Operator = @operator;
            Constraint = constraint;
        }
    }
}
