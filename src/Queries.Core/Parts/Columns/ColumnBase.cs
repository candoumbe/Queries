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
        public abstract bool Equals(ColumnBase other);

        public static implicit operator ColumnBase(int value) => new NumericColumn(value);

        public static implicit operator ColumnBase(double value) => new NumericColumn(value);

        public static implicit operator ColumnBase(float value) => new NumericColumn(value);

        public static implicit operator ColumnBase(string value) => new StringColumn(value);

        public static implicit operator ColumnBase(bool value) => new BooleanColumn(value);

        public static implicit operator ColumnBase(DateTime value) => new DateTimeColumn(value);
    }
}