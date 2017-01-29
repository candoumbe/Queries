using Queries.Core.Parts.Columns;
using System;

namespace Queries.Core.Parts.Functions
{
    /// <summary>
    /// "ISNULL" function
    /// </summary>
    public class NullFunction : IAliasable<NullFunction>, IFunctionColumn
    {
        /// <summary>
        /// 
        /// </summary>
        public IColumn Column { get;  }
        public IColumn DefaultValue { get; }

        /// <summary>
        /// Builds a new <see cref="NullFunction"/> instance.
        /// </summary>
        /// <param name="column">The column to apply the function onto.</param>
        /// <param name="defaultValue">The default value value to use if <paramref name="column"/>'s value is <c>null</c>.</param>
        /// <exception cref="ArgumentNullException"> if either <paramref name="column"/> or <paramref name="defaultValue"/> is <c>null</c></exception>
        public NullFunction(IColumn column, IColumn defaultValue)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }
            if (defaultValue == null)
            {
                throw new ArgumentNullException(nameof(defaultValue));
            }
            Column = column;
            DefaultValue = defaultValue;
        }

        private string _alias;

        public string Alias => _alias;

        /// <summary>
        /// Sets the alias of the result of the function
        /// </summary>
        /// <param name="alias">the new alias</param>
        /// <returns>The current instance</returns>
        public NullFunction As(string alias)
        {
            _alias = alias;

            return this;
        }

    }
}