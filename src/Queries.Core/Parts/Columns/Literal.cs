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
            switch (value)
            {
                case int i:
                    Value = i;
                    break;
                case float f:
                    Value = f;
                    break;
                case double d:
                    Value = d;
                    break;
                case long l:
                    Value = l;
                    break;
                case bool b:
                    Value = b;
                    break;
                case string s:
                    Value = s;
                    break;
                case DateTime dateTime:
                    Value = dateTime;
                    break;
                case DateTimeOffset dateTimeOffset:
                    Value = dateTimeOffset;
                    break;
                case null:
                    Value = value;
                    break;
                default:
                    throw new ArgumentException(nameof(value), "only bool/int/float/double/long/");
            }
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
