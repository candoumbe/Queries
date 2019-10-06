using Queries.Core.Builders;
using System;
using System.Collections.Generic;

namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// A column that render is content as is with no interpretation.
    /// </summary>
    public class Literal : ColumnBase, IAliasable<Literal>, IInsertable, IEquatable<Literal>
    {
        public object Value { get; }

        /// <summary>
        /// Builds a <see cref="Literal"/> instance.
        /// </summary>
        /// <param name="value"></param>
        public Literal(object value = null)
        {
            Value = value switch
            {
                int i => i,
                float f => f,
                double d => d,
                long l => l,
                bool b => b,
                string s => s,
                DateTime dateTime => dateTime,
                DateTimeOffset dateTimeOffset => dateTimeOffset,
                null => value,
                _ => throw new ArgumentException(nameof(value), "only bool/int/float/double/long/string/Datetime/DateTimeOffset are supported"),
            };
        }

        private string _alias;

        public string Alias => _alias;

        public Literal As(string alias)
        {
            _alias = alias;

            return this;
        }

        public override bool Equals(object obj) => Equals(obj as Literal);

        public bool Equals(Literal other) => (Value, Alias).Equals((other?.Value, other?.Alias));

        public override bool Equals(ColumnBase other) => Equals(other as Literal);

        public override int GetHashCode() => (Value, Alias).GetHashCode();

        public override IColumn Clone() => new Literal(Value);

    }
}
