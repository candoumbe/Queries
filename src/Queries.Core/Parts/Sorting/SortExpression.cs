using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Sorting
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