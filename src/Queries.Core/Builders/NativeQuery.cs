﻿using System;

namespace Queries.Core.Builders;

/// <summary>
/// A query that will be rendered "as is".
/// </summary>
#if NET5_0_OR_GREATER
public record NativeQuery : IQuery
#elif NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_3 || NETSTANDARD2_1
public class NativeQuery : IQuery, IEquatable<NativeQuery>
#else
#error "Unsupported target framework"
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
#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_3 || NETSTANDARD2_1
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
