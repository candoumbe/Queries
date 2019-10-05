using System;

namespace Queries.Core.Parts.Columns
{
    public class StringColumn : Literal, IEquatable<StringColumn>
    {
        public StringColumn(string value = "")
            : base(value)
        {}

        public bool Equals(StringColumn other) => (Value, Alias).Equals((other?.Value, other?.Alias));

        public override bool Equals(object obj) => Equals(obj as StringColumn);

#if !NETSTANDARD2_1
        public override int GetHashCode() => (Value, Alias).GetHashCode();
#else
        public override int GetHashCode() => HashCode.Combine(Value, Alias);
#endif

        public override string ToString() => this.Jsonify();
    }
}