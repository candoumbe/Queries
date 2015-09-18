namespace Queries.Parts.Columns
{
    public class Concat : ConcatColumn
    {
        public Concat(params IColumn[] columns) : base(columns)
        {}
    }
}