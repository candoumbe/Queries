using Queries.Parts.Columns;

namespace Queries.Parts.Clauses
{
    public class WhereClause : IWhereClause, IClause<IColumn>
    {
        public IColumn Column { get; set; }
        public ClauseOperator Operator{ get; set; }
        public ColumnBase Constraint { get; set; }
        
    }
}
