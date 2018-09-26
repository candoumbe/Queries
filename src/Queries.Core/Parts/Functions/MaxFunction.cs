using System;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Functions
{
    /// <summary>
    /// "MAX" fuction
    /// </summary>
    public class MaxFunction : AggregateFunction
    {
        /// <summary>
        /// Buils a new <see cref="MaxFunction"/> instance.
        /// </summary>
        /// <param name="column">The column the function will be applied onto</param>
        /// <exception cref="System.ArgumentNullException">if <paramref name="column"/> is <c>null</c>.</exception>
        public MaxFunction(IColumn column)
            : base(AggregateType.Max, column)
        { }

        /// <summary>
        /// Buils a new <see cref="MaxFunction"/> instance. 
        /// </summary>
        /// <param name="columnName">name of the column onto which the function will be applied.</param>
        /// <exception cref="System.ArgumentNullException">if <paramref name="columnName"/> is <c>null</c></exception>
        public MaxFunction(string columnName) : this(columnName?.Field())
        {

        }
        /// <summary>
        /// Performs a deep copy of the current instance.
        /// </summary>
        /// <returns><see cref="MaxFunction"/></returns>
        public override IColumn Clone() => new MaxFunction(Column.Clone());
    }
}