using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;

namespace Queries.Core.Parts.Clauses
{
    public class HavingClause : IHavingClause, IClause<AggregateColumn>
    {
        public AggregateColumn Column { get; set; }
        public ClauseOperator Operator { get; set; }
        public ColumnBase Constraint { get; set; }
    }
}