using Queries.Core.Parts.Functions.Math;
using Queries.Core.Parts.Sorting;

namespace Queries.Core.Parts.Columns;

/// <summary>
/// Extensions methods for <see cref="ColumnBase"/> type
/// </summary>
public static class ColumnBaseExtensions
{
    /// <summary>
    /// Creates a directive to order <paramref name="column"/> ascending
    /// </summary>
    /// <param name="column"></param>
    /// <returns>The created <see cref="IOrder"/></returns>
    public static IOrder Asc(this ColumnBase column) => new OrderExpression(column);

    /// <summary>
    /// Creates a directive to order <paramref name="column"/> ascending
    /// </summary>
    /// <param name="column"></param>
    /// <returns>The created <see cref="IOrder"/></returns>
    public static IOrder Desc(this ColumnBase column) => new OrderExpression(column, OrderDirection.Descending);

    /// <summary>
    /// Builds a <see cref="SubstractFunction"/> operation
    /// </summary>
    /// <param name="left">left operand</param>
    /// <param name="right">right operand</param>
    /// <returns></returns>
    public static SubstractFunction Substract(this ColumnBase left, IColumn right) => new (left, right);
}
