using Queries.Core.Parts.Columns;

using System;

namespace Queries.Core.Parts.Clauses;

/// <summary>
/// a criterion to apply to a <see cref="Column"/>.
/// </summary>
public class WhereClause : IWhereClause, IClause<IColumn>, IEquatable<WhereClause>
{
    /// <summary>
    /// Unique identifier of the clause
    /// </summary>
    public Guid UniqueId { get; }

    /// <summary>
    /// <see cref="IColumn"/> which the current clause will be applied onto
    /// </summary>
    public IColumn Column { get; }

    /// <summary>
    /// The <see cref="ClauseOperator"/> of the clause
    /// </summary>
    public ClauseOperator Operator{ get; }

    /// <summary>
    /// The constraint of the clause
    /// </summary>
    public IColumn Constraint { get; set; }

    /// <summary>
    /// Builds a new <see cref="WhereClause"/> instance.
    /// </summary>
    /// <param name="column"><see cref="IColumn"/> where to apply the clause onto</param>
    /// <param name="operator"><see cref="ClauseOperator"/> to apply</param>
    /// <param name="constraint">constraint to apply to <paramref name="column"/>.</param>
    /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <see langword="null" />.</exception>
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
    /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <see langword="null" />.</exception>
    public WhereClause(IColumn column, ClauseOperator @operator, string constraint) : this(column, @operator, constraint?.Literal())
    {
    }

    /// <summary>
    /// Builds a new <see cref="WhereClause"/> instance with a <see cref="DateTime"/> constraint.
    /// </summary>
    /// <param name="column"><see cref="IColumn"/> where to apply the clause onto</param>
    /// <param name="operator"><see cref="ClauseOperator"/> to apply</param>
    /// <param name="constraint">constraint to apply to <paramref name="column"/>.</param>
    /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <see langword="null" />.</exception>
    public WhereClause(IColumn column, ClauseOperator @operator, DateTime? constraint) : this(column, @operator, constraint?.Literal())
    {
    }

#if NET6_0_OR_GREATER
    /// <summary>
    /// Builds a new <see cref="WhereClause"/> instance with a <see cref="DateOnly"/> constraint.
    /// </summary>
    /// <param name="column"><see cref="IColumn"/> where to apply the clause onto</param>
    /// <param name="operator"><see cref="ClauseOperator"/> to apply</param>
    /// <param name="constraint">constraint to apply to <paramref name="column"/>.</param>
    /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <see langword="null" />.</exception>
    public WhereClause(IColumn column, ClauseOperator @operator, DateOnly? constraint) : this(column, @operator, constraint?.Literal())
    {
    }

    /// <summary>
    /// Builds a new <see cref="WhereClause"/> instance with a <see cref="TimeOnly"/> constraint.
    /// </summary>
    /// <param name="column"><see cref="IColumn"/> where to apply the clause onto</param>
    /// <param name="operator"><see cref="ClauseOperator"/> to apply</param>
    /// <param name="constraint">constraint to apply to <paramref name="column"/>.</param>
    /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <see langword="null" />.</exception>
    public WhereClause(IColumn column, ClauseOperator @operator, TimeOnly? constraint) : this(column, @operator, constraint?.Literal())
    {
    }
#endif

    /// <summary>
    /// Builds a new <see cref="WhereClause"/> instance with a <see langword="bool"/> constraint.
    /// </summary>
    /// <param name="column"><see cref="IColumn"/> where to apply the clause onto</param>
    /// <param name="operator"><see cref="ClauseOperator"/> to apply</param>
    /// <param name="constraint">constraint to apply to <paramref name="column"/>.</param>
    /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <see langword="null" />.</exception>
    public WhereClause(IColumn column, ClauseOperator @operator, bool? constraint) : this(column, @operator, constraint?.Literal())
    {
    }

    /// <summary>
    /// Builds a new <see cref="WhereClause"/> instance with a <see cref="long"/> constraint.
    /// </summary>
    /// <param name="column"><see cref="IColumn"/> where to apply the clause onto</param>
    /// <param name="operator"><see cref="ClauseOperator"/> to apply</param>
    /// <param name="constraint">constraint to apply to <paramref name="column"/>.</param>
    /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <see langword="null" />.</exception>
    public WhereClause(IColumn column, ClauseOperator @operator, long? constraint) : this(column, @operator, constraint?.Literal())
    {
    }

    /// <summary>
    /// Builds a new <see cref="WhereClause"/> instance with a <see cref="decimal"/> constraint.
    /// </summary>
    /// <param name="column"><see cref="IColumn"/> where to apply the clause onto</param>
    /// <param name="operator"><see cref="ClauseOperator"/> to apply</param>
    /// <param name="constraint">constraint to apply to <paramref name="column"/>.</param>
    /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <see langword="null" />.</exception>
    public WhereClause(IColumn column, ClauseOperator @operator, decimal? constraint) : this(column, @operator, constraint?.Literal())
    {
    }

    /// <summary>
    /// Builds a new <see cref="WhereClause"/> instance with a <see cref="float"/> constraint.
    /// </summary>
    /// <param name="column"><see cref="IColumn"/> where to apply the clause onto</param>
    /// <param name="operator"><see cref="ClauseOperator"/> to apply</param>
    /// <param name="constraint">constraint to apply to <paramref name="column"/>.</param>
    /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <see langword="null" />.</exception>
    public WhereClause(IColumn column, ClauseOperator @operator, float? constraint) : this(column, @operator, constraint?.Literal())
    {
    }

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as WhereClause);

    ///<inheritdoc/>
    public bool Equals(IWhereClause other) => Equals(other as WhereClause);

    ///<inheritdoc/>
    public bool Equals(WhereClause other)
        => Column.Equals(other?.Column)
        && Operator == other?.Operator
        && ((Constraint == null && other?.Constraint == null) || Constraint.Equals(other.Constraint));

    ///<inheritdoc/>
#if !NETSTANDARD2_1
    public override int GetHashCode() => (Column, Operator, Constraint).GetHashCode();
#else
    public override int GetHashCode() => HashCode.Combine(Column, Operator, Constraint);
#endif
    ///<inheritdoc/>
    public IWhereClause Clone() => new WhereClause(Column.Clone(), Operator, Constraint?.Clone());

    ///<inheritdoc/>
    public override string ToString() => (Column, Operator, Constraint).ToString();
}
