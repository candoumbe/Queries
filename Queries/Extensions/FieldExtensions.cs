using Queries.Parts.Columns;
using Queries.Parts.Sorting;

namespace Queries.Extensions
{
    public static class ColumnBaseExtensions
    {
        public static SortExpression Asc(this ColumnBase field)
        {
            return new SortExpression(field);
        }

        public static SortExpression Asc(this string columnNameOrFieldAlias)
        {
            return new SortExpression(columnNameOrFieldAlias.Field());
        }

        public static SortExpression Desc(this ColumnBase field)
        {
            return new SortExpression(field, SortDirection.Descending);
        }

        public static SortExpression Desc(this string columnNameOrFieldAlias)
        {
            return new SortExpression(columnNameOrFieldAlias.Field(), SortDirection.Descending);
        }
    }
}
