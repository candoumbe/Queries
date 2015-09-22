namespace Queries.Parts.Columns
{
    public class StringColumn : LiteralColumn
    {
        public StringColumn(string value = "", string alias ="") : base(value, alias) 
        {}
    }
}