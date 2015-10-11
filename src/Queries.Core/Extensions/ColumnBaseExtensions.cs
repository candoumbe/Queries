using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Sorting;

namespace Queries.Core.Extensions
{
    public static class ColumnBaseExtensions
    {
        public static ISort Asc(this ColumnBase field)
        {
            return new SortExpression(field);
        }

        public static ISort Asc(this string columnNameOrFieldAlias)
        {
            return new SortExpression(columnNameOrFieldAlias.Field());
        }

        public static ISort Desc(this ColumnBase field)
        {
            return new SortExpression(field, SortDirection.Descending);
        }

        public static ISort Desc(this string columnNameOrFieldAlias)
        {
            return new SortExpression(columnNameOrFieldAlias.Field(), SortDirection.Descending);
        }

        


    }
}
