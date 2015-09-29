using Queries.Core.Extensions;

namespace Queries.Core.Parts.Columns
{
    public class MaxColumn : AggregateColumn
    {
        internal MaxColumn(IColumn column)
            : base(AggregateType.Max, column)
        { }

        internal MaxColumn(string columnName) : this(columnName?.Field())
        {
            
        }
    }
}