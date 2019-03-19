using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Sorting;

namespace Queries.Core.Extensions
{
    public static class ColumnBaseExtensions
    {
        public static IOrder Asc(this ColumnBase field) => new OrderExpression(field);

        public static IOrder Desc(this ColumnBase field) => new OrderExpression(field, OrderDirection.Descending);
    }
}
