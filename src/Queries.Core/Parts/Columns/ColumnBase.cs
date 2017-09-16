using Queries.Core.Extensions;
using System;

namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// Provide implicit cast from primitive types (<see cref="int"/>/<see cref="double"/>/<see cref="float"/> ...)
    /// to <see cref="LiteralColumn"/>.
    /// </summary>
    public class ColumnBase : IColumn
    {
        public static implicit operator ColumnBase(int value) => new NumericColumn(value);

        public static implicit operator ColumnBase(double value) => new NumericColumn(value);

        public static implicit operator ColumnBase(float value) => new NumericColumn(value);

        public static implicit operator ColumnBase(string value) => new StringColumn(value);

    }
}