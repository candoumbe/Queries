namespace Queries.Core.Parts.Columns
{
    public class StringColumn : LiteralColumn
    {
        internal StringColumn(string value = "")
            : base(value) 
        {}
    }
}