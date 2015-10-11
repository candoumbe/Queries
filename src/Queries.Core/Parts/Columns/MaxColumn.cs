using Queries.Core.Extensions;

namespace Queries.Core.Parts.Columns
{
    public class MaxColumn : AggregateColumn
    {
        public MaxColumn(IColumn column)
            : base(AggregateType.Max, column)
        { }

        public MaxColumn(string columnName) : this(columnName?.Field())
        {
            
        }
    }
}