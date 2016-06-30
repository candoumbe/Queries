using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Functions
{
    public class AvgColumn : AggregateColumn
    {
        public AvgColumn(IColumn column)
            : base(AggregateType.Average, column)
        { }


        public AvgColumn(string columnName) : this(columnName?.Field()) { }
    }
}