using System;


namespace Queries.Core.Builders
{
    /// <summary>
    /// A query that will be rendered "as is".
    /// </summary>
    public class NativeQuery : IQuery, IEquatable<NativeQuery>
    {
        public string Statement { get; }

        /// <summary>
        /// Builds a new <see cref="NativeQuery"/> instance.
        /// </summary>
        /// <param name="nativeString"></param>
        public NativeQuery(string nativeString)
        {
            Statement = nativeString;
        }

        public bool Equals(NativeQuery other) => Statement.Equals(other?.Statement);

        public override int GetHashCode() => Statement?.GetHashCode() ?? 0;

        public override bool Equals(object obj) => Equals(obj as NativeQuery);

        public static implicit operator NativeQuery (string statement) => new NativeQuery(statement);
    }
}
