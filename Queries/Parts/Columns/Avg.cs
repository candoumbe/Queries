namespace Queries.Parts.Columns
{
    public class Avg : AggregateColumn
    {
        public Avg(TableColumn column) : base(AggregateType.Average, column)
        { }
    }
}