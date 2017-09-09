namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// Provide implicit cast from primitive types (<see cref="int"/>/<see cref="double"/>/<see cref="float"/> ...)
    /// to <see cref="LiteralColumn"/>.
    /// </summary>
    public class ColumnBase : IColumn
    {
        public static implicit operator ColumnBase(int value) => new LiteralColumn(value);

        public static implicit operator ColumnBase(double value) => new LiteralColumn(value);

        public static implicit operator ColumnBase(float value) => new LiteralColumn(value);

        public static implicit operator ColumnBase(string value) => new LiteralColumn(value);


    }
}