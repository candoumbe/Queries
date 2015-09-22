namespace Queries.Parts.Columns
{
    public class Null : NullColumn
    {
        public Null(IColumn column, IColumn defaultValue, string alias = "")
            : base(column, defaultValue, alias)
        { }
    }

    
}