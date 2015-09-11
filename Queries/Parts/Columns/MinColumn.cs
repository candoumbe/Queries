namespace Queries.Parts.Columns
{
    public class MinColumn : AggregateColumn
    {

        public MinColumn(FieldColumn column, string alias = null)
            : base(AggregateType.Min, column, alias)
        {}

        public MinColumn(string columnName, string alias = null)
            : this(new FieldColumn() { Name = columnName }, alias)
        {}
    }
}