using Queries.Core.Parts.Columns;
using System;

namespace Queries.Core.Parts.Clauses
{
    public class WhereClause : IWhereClause, IClause<IColumn>
    {
        public IColumn Column { get; }
        public ClauseOperator Operator{ get; }
        public ColumnBase Constraint { get; }

        /// <summary>
        /// Builds a new <see cref="WhereClause"/> instance.
        /// </summary>
        /// <param name="column"><see cref="IColumn"/> where to apply the clase onto</param>
        /// <param name="operator"><see cref="ClauseOperator"/> to apply</param>
        /// <param name="constraint">constraint to apply to <paramref name="column"/>.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <c>null</c>.</exception>
        public WhereClause(IColumn column, ClauseOperator @operator, ColumnBase constraint = null)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }
            Column = column;
            Operator = @operator;
            if (@operator != ClauseOperator.IsNull && @operator != ClauseOperator.IsNotNull)
            {
                Constraint = constraint;
            }
        }
    }
}
