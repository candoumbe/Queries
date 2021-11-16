using System;

namespace Queries.Core.Parts.Columns
{
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

        ///<inheritdoc/>
        public static implicit operator ColumnBase(int value) => new NumericColumn(value);

        ///<inheritdoc/>
        public static implicit operator ColumnBase(double value) => new NumericColumn(value);

        ///<inheritdoc/>
        public static implicit operator ColumnBase(float value) => new NumericColumn(value);

        ///<inheritdoc/>
        public static implicit operator ColumnBase(string value) => new StringColumn(value);

        ///<inheritdoc/>
        public static implicit operator ColumnBase(bool value) => new BooleanColumn(value);

        ///<inheritdoc/>
        public static implicit operator ColumnBase(DateTime value) => new DateTimeColumn(value);

#if NET6_0_OR_GREATER
        ///<inheritdoc/>
        public static implicit operator ColumnBase(DateOnly value) => new DateColumn(value);

        ///<inheritdoc/>
        public static implicit operator ColumnBase(TimeOnly value) => new TimeColumn(value);
#endif
    }
}