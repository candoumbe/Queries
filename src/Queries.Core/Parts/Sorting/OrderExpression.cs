using Queries.Core.Parts.Columns;
using System;

namespace Queries.Core.Parts.Sorting
{
    /// <summary>
    /// Represents a ORDER expression
    /// </summary>
    public class OrderExpression : IOrder
    {
        /// <summary>
        /// Column which the sort expression will be applied onto
        /// </summary>
        public ColumnBase Column { get; }

        /// <summary>
        /// The <see cref="OrderDirection"/>.
        /// </summary>
        public OrderDirection Direction { get; }

        /// <summary>
        /// Builds a new <see cref="OrderExpression"/> instance.
        /// </summary>
        /// <param name="column">Column onto which the sort will be applied</param>
        /// <param name="direction">sort direction (<see cref="OrderDirection.Ascending"/> by default).</param>
        /// <exception cref="ArgumentNullException"><paramref name="column"/> is <c>null</c>.</exception>
        public OrderExpression(ColumnBase column, OrderDirection direction = OrderDirection.Ascending)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            Direction = direction;
        }

        /// <summary>
        /// Builds a new <see cref="OrderExpression" /> instance. 
        /// </summary>
        /// <param name="columnNameOrAlias">column name/alias onto wich the sort will be applied.</param>
        /// <param name="direction">sort direction (<see cref="OrderDirection.Ascending"/> by default).</param>
        /// <exception cref="ArgumentNullException"><paramref name="columnNameOrAlias"/> is <c>null</c>.</exception>
        public OrderExpression(string columnNameOrAlias, OrderDirection direction = OrderDirection.Ascending) : this(columnNameOrAlias.Field(), direction)
        {}
    }
}