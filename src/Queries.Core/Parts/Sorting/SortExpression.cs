using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Sorting
{
    /// <summary>
    /// Represents a SORT expression
    /// </summary>
    public class SortExpression : ISort
    {
        public ColumnBase Column { get; set; }

        /// <summary>
        /// The <see cref="SortDirection"/>.
        /// </summary>
        public SortDirection Direction { get; set; }

        // TODO unit tests !!!
        /// <summary>
        /// Builds a new <see cref="SortExpression"/> instance.
        /// </summary>
        /// <param name="column">Column onto which the sort will be applied</param>
        /// <param name="direction">sort direction (<see cref="SortDirection.Ascending"/> by default).</param>
        public SortExpression(ColumnBase column, SortDirection direction = SortDirection.Ascending)
        {
            Column = column;
            Direction = direction;
        }

        /// <summary>
        /// Builds a new <see cref="SortExpression" /> instance. 
        /// </summary>
        /// <param name="columnNameOrAlias">column name/alias onto wich the sort will be applied.</param>
        /// <param name="direction">sort direction (<see cref="SortDirection.Ascending"/> by default).</param>
        public SortExpression(string columnNameOrAlias, SortDirection direction = SortDirection.Ascending) : this(columnNameOrAlias.Field(), direction)
        {}
        
         
    }
}