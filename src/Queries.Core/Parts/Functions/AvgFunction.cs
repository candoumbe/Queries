using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Functions
{
    /// <summary>
    /// The "average" function to apply to a <see cref="IColumn"/>.
    /// </summary>
    public class AvgFunction : AggregateFunction
    {
        /// <summary>
        /// Builds a new <see cref="AvgFunction"/> instance.
        /// </summary>
        /// <param name="column">The column onto which the "average" function will be applied.</param>
        public AvgFunction(IColumn column)
            : base(AggregateType.Average, column)
        { }


        /// <summary>
        /// Builds a new <see cref="AvgFunction"/> instance.
        /// </summary>
        /// <param name="column">The name of the column onto which the "average" function will be applied.</param>
        public AvgFunction(string columnName) : this(columnName?.Field()) { }
    }
}