using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;
using System;
using System.Collections.Generic;

namespace Queries.Core.Parts.Clauses
{
    /// <summary>
    /// Constraint to apply to an <see cref="AggregateFunction"/>
    /// </summary>
    public class HavingClause : IHavingClause, IClause<AggregateFunction>, IEquatable<HavingClause>
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


        public IHavingClause Clone() => new HavingClause(Column, Operator, Constraint);
        public override bool Equals(object obj) => Equals(obj as HavingClause);
        public bool Equals(HavingClause other) => other != null && EqualityComparer<AggregateFunction>.Default.Equals(Column, other.Column) && Operator == other.Operator && EqualityComparer<ColumnBase>.Default.Equals(Constraint, other.Constraint);

        public override int GetHashCode()
        {
            int hashCode = -300605098;
            hashCode = (hashCode * -1521134295) + EqualityComparer<AggregateFunction>.Default.GetHashCode(Column);
            hashCode = (hashCode * -1521134295) + Operator.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<ColumnBase>.Default.GetHashCode(Constraint);
            return hashCode;
        }
    }
}