namespace Queries.Parts.Columns
{
    public class Avg : AggregateColumn
    {
        public Avg(Field column, string alias = null)
            : base(AggregateType.Average, column, alias)
        { }
    }
}