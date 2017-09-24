using Queries.Core.Builders;
using System;
using System.Collections.Generic;

namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// Maps a <see cref="FieldColumn"/> with its <see cref="Value"/> in an <see cref="InsertIntoQuery"/>
    /// </summary>
    public class InsertedValue : IInsertable, IEquatable<InsertedValue>
    {
        /// <summary>
        /// Column where the value will be inserted
        /// </summary>
        public FieldColumn Column { get; }

        /// <summary>
        /// Value to insert
        /// </summary>
        public IColumn Value { get; internal set; }

        /// <summary>
        /// Builds a new <see cref="InsertedValue"/>
        /// </summary>
        /// <param name="column">column where the <paramref name="value"/> will be inserted.</param>
        /// <param name="value">value to insert</param>
        /// <exception cref="ArgumentNullException"><paramref name="column"/> is <c>null</c>.</exception>
        public InsertedValue(FieldColumn column, IColumn value)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            Value = value;
        }

        public override bool Equals(object obj) => Equals(obj as InsertedValue);
        public bool Equals(InsertedValue other) => 
            other != null 
            && Column.Equals(other.Column) 
            && (Value == null && other.Value == null || Value.Equals(other.Value));

        public override int GetHashCode()
        {
            int hashCode = 1372869065;
            hashCode = hashCode * -1521134295 + EqualityComparer<FieldColumn>.Default.GetHashCode(Column);
            hashCode = hashCode * -1521134295 + EqualityComparer<IColumn>.Default.GetHashCode(Value);
            return hashCode;
        }
    }
}
