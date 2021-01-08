using System;


namespace Queries.Core.Builders
{
    /// <summary>
    /// A query that will be rendered "as is".
    /// </summary>
#if NET5_0
    public record NativeQuery : IQuery        
#elif NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_3 || NETSTANDARD2_1
    public class NativeQuery : IQuery, IEquatable<NativeQuery>
#else
#error "Unsupported target framework"
#endif
    {
        public string Statement { get; }

        /// <summary>
        /// Builds a new <see cref="NativeQuery"/> instance.
        /// </summary>
        /// <param name="statement"></param>
        /// <exception cref="ArgumentNullException"><paramref name="statement"/> is <c>null</c></exception>
        public NativeQuery(string statement)
        {
            Statement = statement ?? throw new ArgumentNullException(nameof(statement));
        }

#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_3 || NETSTANDARD2_1
        public bool Equals(NativeQuery other) => Statement.Equals(other?.Statement);

        public override int GetHashCode() => Statement?.GetHashCode() ?? 0;

        public override bool Equals(object obj) => Equals(obj as NativeQuery);
#endif
        public static implicit operator NativeQuery(string statement) => new(statement);
    }
}
