using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Sorting
{
    public interface ISort
    {
        ColumnBase Column { get; set; }
        SortDirection Direction { get; set; }
    }
}
