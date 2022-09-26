using System;
using System.Collections.Generic;

namespace Queries.Core.Parts.Columns;

/// <summary>
/// Column that holds a numeric value.
/// </summary>
/// <remarks>
/// Wraps a numeric value so that it can be used wherever a <see cref="ColumnBase"/> can.
/// </remarks>
public class NumericColumn : Literal, IEquatable<NumericColumn>
{
    /// <summary>
    /// The numeric type that the current column holds
    /// </summary>
    public NumericType NumericType { get; }

    /// <summary>
    /// Builds a new <see cref="NumericColumn"/> instance.
    /// </summary>
    /// <param name="value"></param>
    public NumericColumn(int? value = null) : base(value)
    {
        NumericType = NumericType.Integer;
    }

    /// <summary>
    /// Builds a new <see cref="NumericColumn"/> instance .
    /// </summary>
    /// <param name="value"></param>
    public NumericColumn(float? value = null) : base(value)
    {
        NumericType = NumericType.Float;
    }

    /// <summary>
    /// Builds a new <see cref="NumericColumn"/> instance that holds a <see cref="double"/> value.
    /// </summary>
    /// <param name="value"></param>
    public NumericColumn(double? value = null) : base(value)
    {
        NumericType = NumericType.Float;
    }

    /// <summary>
    /// Builds a new <see cref="NumericColumn"/> instance that holds a <see cref="decimal"/> value.
    /// </summary>
    /// <param name="value"></param>
    public NumericColumn(decimal? value = null) : base(value)
    {
        NumericType = NumericType.Float;
    }

    /// <summary>
    /// Builds a new <see cref="NumericColumn"/> instance that holds a <see cref="long"/> value.
    /// </summary>
    /// <param name="value"></param>
    public NumericColumn(long? value = null) : base(value)
    {
        NumericType = NumericType.Integer;
    }

    ///<inheritdoc />
    public static implicit operator NumericColumn(int value) => new(value);

    ///<inheritdoc/>
    public static bool operator ==(NumericColumn left, NumericColumn right) => EqualityComparer<NumericColumn>.Default.Equals(left, right);

    ///<inheritdoc/>
    public static bool operator !=(NumericColumn left, NumericColumn right) => !(left == right);

    ///<inheritdoc/>
    public bool Equals(NumericColumn other)
    {
        bool equals = false;
        if (other != null && Alias == other.Alias && Value != null && other.Value != null)
        {
            equals = NumericType == NumericType.Integer && other.NumericType == NumericType.Integer
                ? Convert.ToInt64(Value) == Convert.ToInt64(other.Value)
                : Math.Abs(Convert.ToDouble(Value) - Convert.ToDouble(other.Value)) <= double.Epsilon;
        }
        return equals;
    }

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as NumericColumn);

    ///<inheritdoc/>
#if !(NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_3 || NETSTANDARD2_0)
    public override int GetHashCode() => HashCode.Combine(NumericType, Value, Alias);
#else
    public override int GetHashCode()
    {
        int hashCode = 571610262;
        hashCode = (hashCode * -1521134295) + base.GetHashCode();
        hashCode = (hashCode * -1521134295) + NumericType.GetHashCode();
        return hashCode;
    }
#endif
}