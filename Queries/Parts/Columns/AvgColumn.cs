namespace Queries.Parts.Columns
{
    public class AvgColumn : AggregateColumn
    {
        public AvgColumn(FieldColumn column)
            : base(AggregateType.Average, column)
        { }
    }
}