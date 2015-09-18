namespace Queries.Parts.Columns
{
    public class CountColumn : AggregateColumn
    {
        public CountColumn(FieldColumn column, string alias = null)
            : base(AggregateType.Count, column, alias)
        { }
    }
}