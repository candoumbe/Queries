namespace Queries.Core.Parts.Columns
{
    public class MaxColumn : AggregateColumn
    {
        public MaxColumn(FieldColumn column)
            : base(AggregateType.Max, column)
        { }
    }
}