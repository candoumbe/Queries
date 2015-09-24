using System;

namespace Queries.Core.Parts.Columns
{
    public class NullColumn : IAliasable<NullColumn>, IFunctionColumn
    {
        public IColumn Column { get; set; }
        public IColumn DefaultValue { get; set; }
        
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
        public NullColumn(IColumn column, IColumn defaultValue)
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

        public NullColumn As(string alias)
        {
            _alias = alias;

            return this;
        }

    }
}