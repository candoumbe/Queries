namespace Queries.Core.Parts.Columns
{
    public class NumericColumn : LiteralColumn
    {
        internal NumericColumn(int? value = null) : base(value)
        {}
        internal NumericColumn(float? value = null)
            : base(value)
        { }

        internal NumericColumn(double? value = null)
            : base(value)
        { }

        internal NumericColumn(long? value = null)
            : base(value)
        { }
    }
}