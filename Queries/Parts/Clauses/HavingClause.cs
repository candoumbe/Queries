using Queries.Parts.Columns;

namespace Queries.Parts.Clauses
{
    public class HavingClause : IHavingClause, IClause<AggregateColumn>
    {
        public AggregateColumn Column { get; set; }
        public ClauseOperator Operator { get; set; }
        public ColumnBase Constraint { get; set; }
    }
}