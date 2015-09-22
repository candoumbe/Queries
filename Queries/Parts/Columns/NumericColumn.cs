namespace Queries.Parts.Columns
{
    public class NumericColumn : LiteralColumn
    {
        public NumericColumn(int? value = null) : base(value)
        {}
        public NumericColumn(float? value = null)
            : base(value)
        { }

        public NumericColumn(double? value = null)
            : base(value)
        { }

        public NumericColumn(long? value = null)
            : base(value)
        { }
    }
}