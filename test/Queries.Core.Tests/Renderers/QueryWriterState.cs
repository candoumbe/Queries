
using System;
using System.Collections.Generic;

namespace Queries.Core.Tests.Renderers
{
#if NETCOREAPP3_1
    public class QueryWriterState : IEquatable<QueryWriterState>
    {
        public QueryWriterState(string value, int blockLevel, bool prettyPrint)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            BlockLevel = blockLevel;
            PrettyPrint = prettyPrint;
        }

        public string Value { get; }

        public int BlockLevel { get; }

        public bool PrettyPrint { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as QueryWriterState);
        }

        public bool Equals(QueryWriterState other)
        {
            return (Value, BlockLevel, PrettyPrint).Equals((other?.Value, other?.BlockLevel, other?.PrettyPrint));
        }

        public override int GetHashCode() => HashCode.Combine(Value, BlockLevel, PrettyPrint);

        public static bool operator ==(QueryWriterState left, QueryWriterState right)
        {
            return EqualityComparer<QueryWriterState>.Default.Equals(left, right);
        }

        public static bool operator !=(QueryWriterState left, QueryWriterState right)
        {
            return !(left == right);
        }

        public void Deconstruct(out string value, out bool prettyPrint, out int blockLevel)
        {
            prettyPrint = PrettyPrint;
            value = Value;
            blockLevel = BlockLevel;
        }

        public override string ToString() => this.Jsonify();
    }
#elif NET5_0
    public record QueryWriterState(string Value, int BlockLevel, bool PrettyPrint);
#else
#error Unsupported framework
#endif
}
