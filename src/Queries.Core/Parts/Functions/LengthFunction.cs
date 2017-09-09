using Queries.Core.Attributes;
using Queries.Core.Parts.Columns;
using System;

namespace Queries.Core.Parts.Functions
{
    /// <summary>
    /// Function that computes the length of a column
    /// </summary>
    [Function]
    public class LengthFunction : IAliasable<LengthFunction>, IColumn
    {
        /// <summary>
        /// The column onto which <see cref="LengthFunction"/> will be applied
        /// </summary>
        public IColumn Column { get; }
        private string _alias;

        /// <summary>
        /// Alias sets for the function.
        /// </summary>
        public string Alias => _alias;

        /// <summary>
        /// Builds a new <see cref="LengthFunction"/> instance.
        /// </summary>
        /// <param name="column">The column onto which the function must be applied.</param>
        /// <see cref="IFunctionColumn"/>
        /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <c>nulll</c></exception>
        public LengthFunction(IColumn column)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
        }
        
        /// <summary>
        /// Set the alias of the function
        /// </summary>
        /// <param name="alias"></param>
        /// <returns>The current instance</returns>
        public LengthFunction As(string alias)
        {
            _alias = alias;

            return this;
        }
    }
}
