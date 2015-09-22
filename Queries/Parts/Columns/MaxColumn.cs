namespace Queries.Parts.Columns
{
    public class MaxColumn : AggregateColumn
    {
        public MaxColumn(FieldColumn column, string alias = null)
            : base(AggregateType.Max, column, alias)
        { }
    }
}