namespace Queries.Parts.Columns
{
    public class AvgColumn : AggregateColumn
    {
        public AvgColumn(FieldColumn column, string alias = null)
            : base(AggregateType.Average, column, alias)
        { }
    }
}