namespace Queries.Parts.Columns
{
    public class Count : AggregateColumn
    {
        public Count(TableColumn column)
            : base(AggregateType.Count, column)
        { }
    }


    
}