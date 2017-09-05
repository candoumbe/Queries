using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Sorting
{
    public interface ISort
    {
        /// <summary>
        /// Column onto which the sort expression will be applied
        /// </summary>
        ColumnBase Column { get; }

        /// <summary>
        /// Sort direction
        /// </summary>
        SortDirection Direction { get; }
    }
}
