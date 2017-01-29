using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Functions
{
    /// <summary>
    /// Function that computes the length of a column
    /// </summary>
    public class LengthFunction : IAliasable<LengthFunction>, IFunctionColumn
    {
        /// <summary>
        /// The column onto which <see cref="LengthFunction"/> will be applied
        /// </summary>
        public IColumn Column { get; }
        private string _alias;

        public string Alias => _alias;

        /// <summary>
        /// Builds a new <see cref="LengthFunction"/> instance.
        /// </summary>
        /// <param name="column">The column onto which the function must be applied.</param>
        /// <see cref="IFunctionColumn"/>
        internal LengthFunction(IColumn column)
        {
            Column = column;
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
