using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Functions
{
    public class CountColumn : AggregateColumn
    {
        public CountColumn(FieldColumn column)
            : base(AggregateType.Count, column)
        { }

    }
}