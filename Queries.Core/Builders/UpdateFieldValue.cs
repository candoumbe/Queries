using Queries.Core.Parts.Columns;

namespace Queries.Core.Builders
{
    public class UpdateFieldValue
    {
        public ColumnBase Source { get; set; }

        public FieldColumn Destination { get; set; }

        public UpdateFieldValue(FieldColumn destination, ColumnBase source)
        {
            Destination = destination;
            Source = source;
        }
    }
}