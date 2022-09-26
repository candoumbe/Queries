using Queries.Core.Parts.Columns;
using System;

namespace Queries.Core.Parts.Sorting;

/// <summary>
/// Models a <c>Order</c> expression
/// </summary>
public interface IOrder : IEquatable<IOrder>
{
    /// <summary>
    /// Column onto which the order expression will be applied
    /// </summary>
    ColumnBase Column { get; }

    /// <summary>
    /// Sort direction
    /// </summary>
    OrderDirection Direction { get; }
}
