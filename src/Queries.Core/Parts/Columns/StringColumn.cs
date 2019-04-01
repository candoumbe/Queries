using System;

namespace Queries.Core.Parts.Columns
{
    public class StringColumn : Literal, IEquatable<StringColumn>
    {
        internal StringColumn(string value = "")
            : base(value)
        {}


        public bool Equals(StringColumn other) => (Value, Alias).Equals((other?.Value, other?.Alias));

        public override bool Equals(object obj) => Equals(obj as StringColumn);

        public override int GetHashCode() => (Value, Alias).GetHashCode();

        public override string ToString() => this.Stringify();

    }
}