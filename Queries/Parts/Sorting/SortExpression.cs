using Queries.Extensions;
using Queries.Parts.Columns;

namespace Queries.Parts.Sorting
{
    public class SortExpression : ISort
    {
        public ColumnBase Column { get; set; }
        public SortDirection Direction { get; set; }

        public SortExpression(ColumnBase column, SortDirection direction = SortDirection.Ascending)
        {
            Column = column;
            Direction = direction;
        }

        public SortExpression(string columnNameOrAlias, SortDirection direction = SortDirection.Ascending) : this(columnNameOrAlias.Field(), direction)
        {}
        
         
    }
}