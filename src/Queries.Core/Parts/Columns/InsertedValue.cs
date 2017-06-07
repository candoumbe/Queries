using Queries.Core.Builders;
using System;

namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// Maps a <see cref="FieldColumn"/> with its <see cref="Value"/> in an <see cref="InsertIntoQuery"/>
    /// </summary>
    public class InsertedValue : IInsertable
    {
        /// <summary>
        /// Column where the value will be inserted
        /// </summary>
        public FieldColumn Column { get; }

        /// <summary>
        /// Value to insert
        /// </summary>
        public IColumn Value { get; }

        /// <summary>
        /// Builds a new <see cref="InsertedValue"/>
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"><paramref name="column"/> is <c>null</c>.</exception>
        public InsertedValue(FieldColumn column, IColumn value)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            Value = value;
        }
    }
}
