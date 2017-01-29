using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Functions
{
    /// <summary>
    /// "COUNT" function.
    /// </summary>
    public class CountFunction : AggregateFunction
    {
        /// <summary>
        /// Builds a new <see cref="CountFunction"/> instance
        /// </summary>
        /// <param name="column">column onto which the function will bne applied</param>
        /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <c>null</c></exception>
        public CountFunction(FieldColumn column)
            : base(AggregateType.Count, column)
        { }

    }
}