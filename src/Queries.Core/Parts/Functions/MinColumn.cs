using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Functions
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