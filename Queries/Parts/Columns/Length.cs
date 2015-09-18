namespace Queries.Parts.Columns
{
    public class Length : LengthColumn
    {
        public Length(StringColumn column, string alias = null) : base(column, alias)
        { }

        public Length(FieldColumn column, string alias = null) : base(column, alias)
        { }
}
}