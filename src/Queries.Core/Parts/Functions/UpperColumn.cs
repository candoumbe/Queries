using System;
using Queries.Core.Parts.Columns;
using Queries.Core.Extensions;

namespace Queries.Core.Parts.Functions
{
    public class UpperColumn : IFunctionColumn, IAliasable<UpperColumn> 
    {

        public IColumn Column { get; private set; }

        public string Alias { get; private set; }
        
        /// <summary>
        /// Applies the "UPPER" function to the specified column
        /// </summary>
        /// <param name="column">Column the function will be applied on</param>
        /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <code>null</code></exception>
        public UpperColumn(IColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            Column = column;
        }

        /// <summary>
        /// Applies the "UPPER" function to the specified value
        /// </summary>
        /// <param name="value">the value the function will be applied on</param>
        /// <exception cref="ArgumentNullException">if <paramref name="value"/> is <code>null</code></exception>
        public UpperColumn(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }


            Column = value.Literal();

        }

        public UpperColumn As(string alias)
        {
            Alias = alias;

            return this;
        }
    }
}