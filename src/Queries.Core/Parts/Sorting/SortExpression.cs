using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using System;

namespace Queries.Core.Parts.Sorting
{
    /// <summary>
    /// Represents a SORT expression
    /// </summary>
    public class SortExpression : ISort
    {
        /// <summary>
        /// Column which the sort expression will be applied onto
        /// </summary>
        public ColumnBase Column { get; }

        /// <summary>
        /// The <see cref="SortDirection"/>.
        /// </summary>
        public SortDirection Direction { get; }

        /// <summary>
        /// Builds a new <see cref="SortExpression"/> instance.
        /// </summary>
        /// <param name="column">Column onto which the sort will be applied</param>
        /// <param name="direction">sort direction (<see cref="SortDirection.Ascending"/> by default).</param>
        /// <exception cref="ArgumentNullException"><paramref name="column"/> is <c>null</c>.</exception>
        public SortExpression(ColumnBase column, SortDirection direction = SortDirection.Ascending)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            Direction = direction;
        }

        /// <summary>
        /// Builds a new <see cref="SortExpression" /> instance. 
        /// </summary>
        /// <param name="columnNameOrAlias">column name/alias onto wich the sort will be applied.</param>
        /// <param name="direction">sort direction (<see cref="SortDirection.Ascending"/> by default).</param>
        /// <exception cref="ArgumentNullException"><paramref name="columnNameOrAlias"/> is <c>null</c>.</exception>
        public SortExpression(string columnNameOrAlias, SortDirection direction = SortDirection.Ascending) : this(columnNameOrAlias.Field(), direction)
        {}
        
         
    }
}