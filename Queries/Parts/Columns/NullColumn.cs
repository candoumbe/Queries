using System;

namespace Queries.Parts.Columns
{
    public class NullColumn : IFunctionColumn, IAliasable
    {
        public IColumn Column { get; set; }
        public IColumn DefaultValue { get; set; }
        public string Alias { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullColumn"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="alias">The alias.</param>
        /// <exception cref="ArgumentNullException">
        /// column
        /// or
        /// defaultValue
        /// </exception>
        public NullColumn(IColumn column, IColumn defaultValue, string alias = "")
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }
            if (defaultValue == null)
            {
                throw new ArgumentNullException("defaultValue");
            }
            Column = column;
            DefaultValue = defaultValue;
            Alias = alias;
        }


        
    }
}