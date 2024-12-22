using System;

namespace Queries.Core.Builders;

/// <summary>
/// A query that will be rendered "as is".
/// </summary>
#if NET
public record NativeQuery : IQuery
#else
public class NativeQuery : IQuery, IEquatable<NativeQuery>
#endif
{
    /// <summary>
    /// The native query
    /// </summary>
    public string Statement { get; }

    /// <summary>
    /// Builds a new <see cref="NativeQuery"/> instance.
    /// </summary>
    /// <param name="statement"></param>
    /// <exception cref="ArgumentNullException"><paramref name="statement"/> is <see langword="null" /></exception>
    public NativeQuery(string statement)
    {
        Statement = statement ?? throw new ArgumentNullException(nameof(statement));
    }
#if NETSTANDARD
    ///<inheritdoc/>
    public bool Equals(NativeQuery other) => Statement.Equals(other?.Statement);

    ///<inheritdoc/>
    public override int GetHashCode() => Statement?.GetHashCode() ?? 0;

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as NativeQuery);
#endif
    ///<inheritdoc/>
    public static implicit operator NativeQuery(string statement) => new(statement);
}
