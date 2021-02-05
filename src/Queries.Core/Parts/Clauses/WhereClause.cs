using Queries.Core.Parts.Columns;

using System;

namespace Queries.Core.Parts.Clauses
{
    /// <summary>
    /// a criterion to apply to a <see cref="Column"/>.
    /// </summary>
    public class WhereClause : IWhereClause, IClause<IColumn>, IEquatable<WhereClause>
    {
        public Guid UniqueId { get; }

        public IColumn Column { get; }

        public ClauseOperator Operator{ get; }

        public IColumn Constraint { get; set; }

        /// <summary>
        /// Builds a new <see cref="WhereClause"/> instance.
        /// </summary>
        /// <param name="column"><see cref="IColumn"/> where to apply the clause onto</param>
        /// <param name="operator"><see cref="ClauseOperator"/> to apply</param>
        /// <param name="constraint">constraint to apply to <paramref name="column"/>.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <c>null</c>.</exception>
        public WhereClause(IColumn column, ClauseOperator @operator, IColumn constraint = null)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            UniqueId = Guid.NewGuid();
            Operator = @operator;
            if (@operator != ClauseOperator.IsNull && @operator != ClauseOperator.IsNotNull)
            {
                if (@operator == ClauseOperator.In)
                {
                    switch (constraint)
                    {
                        case StringValues strings:
                            Constraint = strings;
                            break;
                        case null:
                            throw new ArgumentNullException(nameof(constraint),
                                $"{nameof(constraint)} cannot be null when {nameof(@operator)} is {nameof(ClauseOperator)}.{nameof(ClauseOperator.In)}");

                        default:

                            break;
                    }
                }
                Constraint = constraint;
            }
        }

        /// <summary>
        /// Builds a new <see cref="WhereClause"/> instance with a <see cref="string"/> constraint.
        /// </summary>
        /// <param name="column"><see cref="IColumn"/> where to apply the clause onto</param>
        /// <param name="operator"><see cref="ClauseOperator"/> to apply</param>
        /// <param name="constraint">constraint to apply to <paramref name="column"/>.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <c>null</c>.</exception>
        public WhereClause(IColumn column, ClauseOperator @operator, string constraint) : this(column, @operator, constraint?.Literal())
        {
        }

        /// <summary>
        /// Builds a new <see cref="WhereClause"/> instance with a <see cref="DateTime"/> constraint.
        /// </summary>
        /// <param name="column"><see cref="IColumn"/> where to apply the clause onto</param>
        /// <param name="operator"><see cref="ClauseOperator"/> to apply</param>
        /// <param name="constraint">constraint to apply to <paramref name="column"/>.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <c>null</c>.</exception>
        public WhereClause(IColumn column, ClauseOperator @operator, DateTime? constraint) : this(column, @operator, constraint?.Literal())
        {
        }

        public WhereClause(IColumn column, ClauseOperator @operator, bool? constraint) : this(column, @operator, constraint?.Literal())
        {
        }

        /// <summary>
        /// Builds a new <see cref="WhereClause"/> instance with a <see cref="long"/> constraint.
        /// </summary>
        /// <param name="column"><see cref="IColumn"/> where to apply the clause onto</param>
        /// <param name="operator"><see cref="ClauseOperator"/> to apply</param>
        /// <param name="constraint">constraint to apply to <paramref name="column"/>.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <c>null</c>.</exception>
        public WhereClause(IColumn column, ClauseOperator @operator, long? constraint) : this(column, @operator, constraint?.Literal())
        {
        }

        /// <summary>
        /// Builds a new <see cref="WhereClause"/> instance with a <see cref="decimal"/> constraint.
        /// </summary>
        /// <param name="column"><see cref="IColumn"/> where to apply the clause onto</param>
        /// <param name="operator"><see cref="ClauseOperator"/> to apply</param>
        /// <param name="constraint">constraint to apply to <paramref name="column"/>.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <c>null</c>.</exception>
        public WhereClause(IColumn column, ClauseOperator @operator, decimal? constraint) : this(column, @operator, constraint?.Literal())
        {
        }

        /// <summary>
        /// Builds a new <see cref="WhereClause"/> instance with a <see cref="float"/> constraint.
        /// </summary>
        /// <param name="column"><see cref="IColumn"/> where to apply the clause onto</param>
        /// <param name="operator"><see cref="ClauseOperator"/> to apply</param>
        /// <param name="constraint">constraint to apply to <paramref name="column"/>.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <c>null</c>.</exception>
        public WhereClause(IColumn column, ClauseOperator @operator, float? constraint) : this(column, @operator, constraint?.Literal())
        {
        }

        public override bool Equals(object obj) => Equals(obj as WhereClause);

        public bool Equals(IWhereClause other) => Equals(other as WhereClause);

        public bool Equals(WhereClause other)
            => Column.Equals(other?.Column)
            && Operator == other?.Operator
            && ((Constraint == null && other?.Constraint == null) || Constraint.Equals(other.Constraint));

#if !NETSTANDARD2_1
        public override int GetHashCode() => (Column, Operator, Constraint).GetHashCode();
#else
        public override int GetHashCode() => HashCode.Combine(Column, Operator, Constraint);
#endif
        public IWhereClause Clone() => new WhereClause(Column.Clone(), Operator, Constraint?.Clone());

        public override string ToString() => (Column, Operator, Constraint).ToString();
    }
}
