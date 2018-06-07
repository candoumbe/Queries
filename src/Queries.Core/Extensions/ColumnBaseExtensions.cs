using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions.Math;
using Queries.Core.Parts.Sorting;

namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// Extensions methods for <see cref="ColumnBase"/> type
    /// </summary>
    public static class ColumnBaseExtensions
    {
        public static ISort Asc(this ColumnBase field) => new SortExpression(field);

        
        public static ISort Desc(this ColumnBase field) => new SortExpression(field, SortDirection.Descending);

        /// <summary>
        /// Builds a <see cref="SubstractFunction"/> operation
        /// </summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns></returns>
        public static SubstractFunction Substract(this ColumnBase left, IColumn right) => new SubstractFunction(left, right);
        
    }
}
