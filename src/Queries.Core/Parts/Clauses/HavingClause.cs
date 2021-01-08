using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;
using System;

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
        public ClauseOperator Operator { get; }

        /// <summary>
        /// Constraint to apply
        /// </summary>
        public IColumn Constraint { get; }

        /// <summary>
        /// Builds a new <see cref="HavingClause"/> instance.
        /// </summary>
        /// <param name="column">column the </param>
        /// <param name="operator"></param>
        /// <param name="constraint"></param>
        public HavingClause(AggregateFunction column, ClauseOperator @operator, IColumn constraint = null)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            Operator = @operator;
            Constraint = constraint;
        }

        public HavingClause(AggregateFunction column, ClauseOperator @operator, string constraint)
            : this(column, @operator, constraint?.Literal())
        {
        }

        public HavingClause(AggregateFunction column, ClauseOperator @operator, long? constraint)
            : this(column, @operator, constraint?.Literal())
        {
        }

        public HavingClause(AggregateFunction column, ClauseOperator @operator, bool? constraint)
            : this(column, @operator, constraint?.Literal())
        {
        }

        public IHavingClause Clone() => new HavingClause(Column, Operator, Constraint);

        public override bool Equals(object obj) => Equals(obj as HavingClause);

        public bool Equals(HavingClause other) => other != null
                                                  && (Column, Operator, Constraint).Equals((other.Column, other.Operator, other.Constraint));

        public override int GetHashCode() => (Column, Operator, Constraint).GetHashCode();
    }
}