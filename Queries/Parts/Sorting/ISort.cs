using Queries.Parts.Columns;

namespace Queries.Parts.Sorting
{
    public interface ISort
    {
        ColumnBase Column { get; set; }
        SortDirection Direction { get; set; }
    }
}
