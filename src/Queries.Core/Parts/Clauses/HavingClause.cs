using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;
using System;

namespace Queries.Core.Parts.Clauses
{
    /// <summary>
    /// Constraint to apply to an <see cref="AggregateFunction"/>
    /// </summary>
    public class HavingClause : IHavingClause, IClause<AggregateFunction>
    {
        /// <summary>
        /// Column the clause will be applied onto
        /// </summary>
        public AggregateFunction Column { get; }
        /// <summary>
        /// Operator to apply beetwen <see cref="Column"/> and <see cref="Constraint"/>.
        /// </summary>
        public ClauseOperator Operator { get;  }

        /// <summary>
        /// Constraint to apply
        /// </summary>
        public ColumnBase Constraint { get; }

        /// <summary>
        /// Builds a new <see cref="HavingClause"/> instance.
        /// </summary>
        /// <param name="column">column the </param>
        /// <param name="operator"></param>
        /// <param name="constraint"></param>
        public HavingClause(AggregateFunction column, ClauseOperator @operator, ColumnBase constraint = null)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            Operator = @operator;
            Constraint = constraint;
        }
    }
}