namespace Queries.Parts.Columns
{
    public class CountColumn : AggregateColumn
    {
        public CountColumn(FieldColumn column)
            : base(AggregateType.Count, column)
        { }
    }
}