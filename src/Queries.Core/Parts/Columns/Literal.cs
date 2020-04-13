using Queries.Core.Builders;
using System;

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
                decimal d => d,
                double d => d,
                long l => l,
                bool b => b,
                string s => s,
                DateTime dateTime => dateTime,
                DateTimeOffset dateTimeOffset => dateTimeOffset,
                null => null,
                _ => throw new ArgumentException(nameof(value), "only bool/int/float/decimal/double/long/string/Datetime/DateTimeOffset are supported"),
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

        public bool Equals(Literal other) => other != null
            && (other.Value?.Equals(Value) ?? false)
            && (Alias == other.Alias);

        public override bool Equals(ColumnBase other) => Equals(other as Literal);

        public override int GetHashCode() => (Value, Alias).GetHashCode();

        public override IColumn Clone() => new Literal(Value);

        public override string ToString() => this.Jsonify();

        public static implicit operator Literal(bool value) => new BooleanColumn(value);

        public static implicit operator Literal(string value) => new StringColumn(value);

        public static implicit operator Literal(int value) => new NumericColumn(value);

        public static implicit operator Literal(float value) => new NumericColumn(value);

        public static implicit operator Literal(short value) => new NumericColumn(value);

        public static implicit operator Literal(long value) => new NumericColumn(value);
    }
}
