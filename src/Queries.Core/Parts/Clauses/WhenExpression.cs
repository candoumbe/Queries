using Queries.Core.Parts.Columns;

using System;
using System.Collections.Generic;

namespace Queries.Core.Parts.Clauses;

/// <summary>
/// Expression to use in a `CASE` statement
/// </summary>
public class WhenExpression : IEquatable<WhenExpression>
{
    /// <summary>
    /// Criterion of the expression
    /// </summary>
    public WhereClause Criterion { get; }

    /// <summary>
    /// Value to use when <see cref="Criterion"/> is <c>false</c>.
    /// </summary>
    public ColumnBase ThenValue { get; set; }


    /// <summary>
    /// Builds a new <see cref="WhenExpression"/> instance.
    /// </summary>
    /// <param name="criterion">criterion</param>
    /// <param name="then">value to output when <paramref name="criterion"/> returns <see langword="true"/></param>
    /// <exception cref="ArgumentNullException"><paramref name="criterion"/> is <see langword="null"/> </exception>
    public WhenExpression(WhereClause criterion, ColumnBase then)
    {
        Criterion = criterion ?? throw new ArgumentNullException(nameof(criterion));
        ThenValue = then;
    }

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as WhenExpression);

    ///<inheritdoc/>
    public bool Equals(WhenExpression other) => other is not null 
                                                && EqualityComparer<WhereClause>.Default.Equals(Criterion, other.Criterion) && EqualityComparer<ColumnBase>.Default.Equals(ThenValue, other.ThenValue);

    ///<inheritdoc/>
    public override int GetHashCode()
    {
        int hashCode = -493889257;
        hashCode = hashCode * -1521134295 + EqualityComparer<WhereClause>.Default.GetHashCode(Criterion);
        hashCode = hashCode * -1521134295 + EqualityComparer<ColumnBase>.Default.GetHashCode(ThenValue);
        return hashCode;
    }

    /// <summary>
    /// Tests if 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns><see langword="true"/> when <paramref name="left"/> and <paramref name="right"/> are equal.</returns>
    public static bool operator ==(WhenExpression left, WhenExpression right) => EqualityComparer<WhenExpression>.Default.Equals(left, right);

    /// <summary>
    /// Tests if <paramref name="left"/> is not equal to <paramref name="right"/>
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns><see langword="true"/> when <paramref name="left"/> is not equal to <paramref name="right"/> and <see langword="false"/> otherwise.</returns>
    public static bool operator !=(WhenExpression left, WhenExpression right) => !(left == right);
}
