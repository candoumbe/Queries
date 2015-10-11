using Queries.Core.Extensions;

namespace Queries.Core.Parts.Columns
{
    public class AvgColumn : AggregateColumn
    {
        public AvgColumn(IColumn column)
            : base(AggregateType.Average, column)
        { }


        public AvgColumn(string columnName) : this(columnName?.Field()) { }
    }
}