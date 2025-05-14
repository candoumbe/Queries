using System;

namespace Queries.Core.Parts.Columns;

/// <summary>
/// Provide implicit cast from primitive types (<see cref="int"/>/<see cref="double"/>/<see cref="float"/> ...)
/// to <see cref="Literal"/>.
/// </summary>
public abstract class ColumnBase : IColumn, IEquatable<ColumnBase>
{
    /// <summary>
    /// Should perform a deep copy of the current instance
    /// </summary>
    /// <returns></returns>
    public abstract IColumn Clone();

    ///<inheritdoc/>
    public abstract bool Equals(ColumnBase other);

    /// <summary>
    /// Provides the ability to define type conversion operators for custom casting to <see cref="ColumnBase"/>.
    /// </summary>
    public static implicit operator ColumnBase(int value) => new NumericColumn(value);

    /// <summary>
    /// Allows implicit conversion from double to <see cref="ColumnBase"/>.
    /// </summary>
    /// <param name="value">The double value to convert.</param>
    /// <returns>
    /// A new instance of <see cref="NumericColumn"/> representing the double value.
    /// </returns>
    public static implicit operator ColumnBase(double value) => new NumericColumn(value);

    /// <summary>
    /// Defines the implicit cast operator from the <see cref="float"/> type to a <see cref="ColumnBase"/> object.
    /// </summary>
    /// <param name="value">The floating-point value to cast.</param>
    /// <returns>An instance of <see cref="NumericColumn"/> representing the given value.</returns>
    public static implicit operator ColumnBase(float value) => new NumericColumn(value);

#if NET8_0_OR_GREATER
    /// <summary>
    /// Defines an operator that enables implicit casting to <see cref="ColumnBase"/> from various types such as
    /// <see cref="int"/>, <see cref="string"/>, <see cref="bool"/>, <see cref="double"/>, <see cref="float"/>,
    /// <see cref="DateTime"/>, <see cref="DateOnly"/>, and <see cref="TimeOnly"/>.
    /// </summary>
#else
    /// <summary>
    /// Defines an operator that enables implicit casting to <see cref="ColumnBase"/> from various types such as
    /// <see cref="int"/>, <see cref="string"/>, <see cref="bool"/>, <see cref="double"/>, <see cref="float"/>
    /// </summary>
#endif
    public static implicit operator ColumnBase(string value) => new StringColumn(value);

    /// <summary>
    /// Defines a custom implicit conversion operator for a specific source or target type.
    /// Enables seamless type conversion between primitive types and <see cref="ColumnBase"/> or its derived types.
    /// </summary>
    public static implicit operator ColumnBase(bool value) => new BooleanColumn(value);

    ///<inheritdoc/>
    public static implicit operator ColumnBase(DateTime value) => new DateTimeColumn(value);

#if NET8_0_OR_GREATER
    ///<inheritdoc/>
    public static implicit operator ColumnBase(DateOnly value) => new DateColumn(value);

    ///<inheritdoc/>
    public static implicit operator ColumnBase(TimeOnly value) => new TimeColumn(value);
#endif
}