using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Sorting;

namespace Queries.Core.Extensions
{
    public static class ColumnBaseExtensions
    {
        public static ISort Asc(this ColumnBase field) => new SortExpression(field);

        
        public static ISort Desc(this ColumnBase field) => new SortExpression(field, SortDirection.Descending);

        



    }
}
