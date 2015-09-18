namespace Queries.Parts.Columns
{
    public class NumericColumn : LiteralColumn
    {
        public NumericColumn(int? value = null, string alias = "") : base(value, alias)
        {}
        public NumericColumn(float? value = null, string alias = "")
            : base(value, alias)
        { }

        public NumericColumn(double? value = null, string alias = "")
            : base(value, alias)
        { }

        public NumericColumn(long? value = null, string alias = "")
            : base(value, alias)
        { }
    }
}