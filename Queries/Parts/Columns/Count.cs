namespace Queries.Parts.Columns
{
    public class Count : CountColumn
    {
        public Count(FieldColumn column, string alias = null)
            : base(column, alias)
        {}
    }
}