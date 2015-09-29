using Queries.Core.Extensions;

namespace Queries.Core.Parts.Columns
{
    public class AvgColumn : AggregateColumn
    {
        internal AvgColumn(IColumn column)
            : base(AggregateType.Average, column)
        { }


        internal AvgColumn(string columnName) : this(columnName?.Field()) { }
    }
}