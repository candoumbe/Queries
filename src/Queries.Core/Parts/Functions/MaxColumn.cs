using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Functions
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