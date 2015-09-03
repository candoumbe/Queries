namespace Queries.Parts.Columns
{
    public class Max : AggregateColumn
    {
        public Max(TableColumn column)
            : base(AggregateType.Max, column)
        { }
    }
}