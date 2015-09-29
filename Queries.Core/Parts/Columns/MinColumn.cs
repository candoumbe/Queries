using Queries.Core.Extensions;

namespace Queries.Core.Parts.Columns
{
    public class MinColumn : AggregateColumn
    {

        internal MinColumn(IColumn column)
            : base(AggregateType.Min, column)
        {}

        internal MinColumn(string columnName)
            : this(columnName?.Field())
        {}
    }
}