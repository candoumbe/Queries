using Queries.Parts.Columns;

namespace Queries.Builders
{
    public class UpdateFieldValue
    {
        public ColumnBase Source { get; set; }

        public TableColumn Destination { get; set; }
    }
}