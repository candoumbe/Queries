using Queries.Extensions;

namespace Queries.Parts.Columns
{
    public class MinColumn : AggregateColumn
    {

        public MinColumn(FieldColumn column)
            : base(AggregateType.Min, column)
        {}

        public MinColumn(string columnName)
            : this(columnName.Field())
        {}
    }
}