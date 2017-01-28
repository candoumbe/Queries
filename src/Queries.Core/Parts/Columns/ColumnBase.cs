namespace Queries.Core.Parts.Columns
{
    public class ColumnBase : IColumn
    {
        public static implicit operator ColumnBase(int value) => new LiteralColumn(value);

        public static implicit operator ColumnBase(double value) => new LiteralColumn(value);

        public static implicit operator ColumnBase(float value) => new LiteralColumn(value);

        public static implicit operator ColumnBase(string value) => new LiteralColumn(value);


    }
}