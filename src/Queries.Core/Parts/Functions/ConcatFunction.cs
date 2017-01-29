using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;

namespace Queries.Core.Parts.Functions
{
    /// <summary>
    /// Function which concat to or more columns.
    /// </summary>
    public class ConcatFunction : IAliasable<ConcatFunction>, IFunctionColumn
    {
        /// <summary>
        /// List of columns to concat
        /// </summary>
        public IEnumerable<IColumn> Columns { get; }

        /// <summary>
        /// Builds a new <see cref="ConcatFunction"/> instance.
        /// </summary>
        /// <param name="columns">Columns to concat</param>
        /// <exception cref="ArgumentNullException">if <paramref name="columns"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="columns"/> contains less than two elements.</exception>
        public ConcatFunction(params IColumn[] columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException();
            }
            if (columns.Length < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(columns), $"{nameof(columns)} must be at least two columns to concatenate");
            }
            Columns = columns;
        }

        private string _alias;

        /// <summary>
        /// Alias of the result of the result of the function.
        /// </summary>
        public string Alias => _alias;

        /// <summary>
        /// Gives the result of the concat an alias
        /// </summary>
        /// <param name="alias">The new alias</param>
        /// <returns>The current instance</returns>
        public ConcatFunction As(string alias)
        {
            _alias = alias;

            return this;
        }
    }
}