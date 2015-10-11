using Queries.Core.Extensions;

namespace Queries.Core.Parts.Columns
{
    public class MinColumn : AggregateColumn
    {

        public MinColumn(IColumn column)
            : base(AggregateType.Min, column)
        {}

        public MinColumn(string columnName)
            : this(columnName?.Field())
        {}
    }
}