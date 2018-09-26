using System;
using Queries.Core.Parts.Columns;
using Queries.Core.Attributes;

namespace Queries.Core.Parts.Functions
{
    /// <summary>
    /// "MIN" function.
    /// </summary>
    [Function]
    public class MinFunction : AggregateFunction
    {
        /// <summary>
        /// Buils a new <see cref="MinFunction"/> instance.
        /// </summary>
        /// <param name="column">The column the function will be applied onto</param>
        /// <exception cref="System.ArgumentNullException">if <paramref name="column"/> is <c>null</c>.</exception>
        public MinFunction(IColumn column)
            : base(AggregateType.Min, column)
        { }

        /// <summary>
        /// Buils a new <see cref="MinFunction"/> instance.
        /// </summary>
        /// <param name="columnName">The column the function will be applied onto</param>
        /// <exception cref="System.ArgumentNullException">if <paramref name="columnName"/> is <c>null</c>.</exception>
        public MinFunction(string columnName)
            : this(columnName?.Field())
        { }

        /// <summary>
        /// Performs a deep copy of the current instance.
        /// </summary>
        /// <returns><see cref="MinFunction"/></returns>
        public override IColumn Clone() => new MinFunction(Column.Clone());
    }
}