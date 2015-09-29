namespace Queries.Core.Parts.Columns
{
    public class CountColumn : AggregateColumn
    {
        internal CountColumn(FieldColumn column)
            : base(AggregateType.Count, column)
        { }
    }
}