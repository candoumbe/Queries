namespace Queries.Core.Parts.Columns
{
    public class ColumnBase : IColumn
    {
        public static implicit operator ColumnBase(int value)
        {
            return new LiteralColumn(value);
        }

        public static implicit operator ColumnBase(double value)
        {
            return new LiteralColumn(value);
        }

        public static implicit operator ColumnBase(float value)
        {
            return new LiteralColumn(value);
        }

        public static implicit operator ColumnBase(string value)
        {
            return new LiteralColumn(value);
        }

       
    }
}