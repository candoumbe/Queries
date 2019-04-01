using System;
using Queries.Core.Parts.Columns;
using Queries.Core.Attributes;

namespace Queries.Core.Parts.Functions
{
    /// <summary>
    /// "SUBSTRING" function.
    /// </summary>
    [Function]
    public class SubstringFunction : IColumn, IAliasable<SubstringFunction>
    {
        /// <summary>
        /// Column the function will be applied to
        /// </summary>
        public IColumn Column { get;  }

        /// <summary>
        /// Defines where the substring extraction will start
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// Defines the length of the extracted substring
        /// </summary>
        public int? Length { get; }

        /// <summary>
        /// Builds a new <see cref="SubstringFunction"/> instance.
        /// </summary>
        /// <param name="column">Column onto which the </param>
        /// <param name="start">index of the position where to start the substring</param>
        /// <param name="length">positive integer</param>
        /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">if either :
        ///     - <paramref name="start"/> is less than <c>0</c> <c>null</c>,
        ///     - <paramref name="length"/> is less than <c>0</c>.
        /// </exception>
        public SubstringFunction(IColumn column, int start, int? length = null)
        {
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(start), $"{nameof(start)} must be greater or equal to 0");
            }

            if (length.HasValue && length.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), $"{nameof(length)} must be greater or equal to 0");
            }

            Column = column ?? throw new ArgumentNullException(nameof(column), $"{nameof(column)} cannot be null");
            Start = start;
            Length = length;
        }

        public string Alias { get; private set; }

        /// <summary>
        /// Defines an alias for the <see cref="SubstringFunction"/>
        /// </summary>
        /// <param name="alias">The new alias</param>
        /// <returns></returns>
        public SubstringFunction As(string alias)
        {
            // TODO Validate the alias ?
            Alias = alias;
            return this;
        }



        /// <summary>
        /// Performs a deep copy of the current instance.
        /// </summary>
        /// <returns><see cref="SubstringFunction"/></returns>
        public IColumn Clone() => new SubstringFunction(Column.Clone(), Start, Length);
    }
}
