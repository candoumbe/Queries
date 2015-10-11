using System;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Columns
{
    public class SubstringColumn : IFunctionColumn, IAliasable<SubstringColumn>
    {
        /// <summary>
        /// Gets/Sets the column the function will be applied to
        /// </summary>
        public IColumn Column { get; set; }

        /// <summary>
        /// Defines where the substring extraction will start
        /// </summary>
        public int Start { get; set; }

        public int? Length { get; set; }

        /// <summary>
        /// Creates a new SubstringColumn based on the
        /// </summary>
        /// <param name="column"></param>
        /// <param name="start">index of the position where to start the substring</param>
        /// <param name="length">positive integer</param>
        public SubstringColumn(IColumn column, int start, int? length = null)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column), $"{nameof(column)} cannot be null");
            }

            if (start < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(start), $"{nameof(start)} must be greater or equal to 0");
            }

            if (length.HasValue && length.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), $"{nameof(length)} must be greater or equal to 0");
            }

            Column = column;
            Start = start;
            Length = length;
        }

        public string Alias { get; private set; }
        public SubstringColumn As(string alias)
        {
            Alias = alias;
            return this;
        }
    }
}
