namespace Queries.Parts.Columns
{
    public class Min : MinColumn
    {


        public Min(FieldColumn column, string alias = null)
            : base(column, alias)
        {}

        public Min(string columnName, string alias = null)
            : base(columnName, alias)
        {}
    }
}