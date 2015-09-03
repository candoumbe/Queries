namespace Queries.Parts.Columns
{
    public class Min : AggregateColumn
    {
        public Min(TableColumn column) : base(AggregateType.Min, column)
        {}

        public Min(string columnName) : this(new TableColumn(){Name = columnName})
        {}
    }
}