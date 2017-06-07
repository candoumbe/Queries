using Queries.Core.Parts.Columns;

namespace Queries.Core.Builders
{
    /// <summary>
    /// Maps a <see cref="FieldColumn"/> with a new value.
    /// Used in <see cref="UpdateQuery"/>
    /// </summary>
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