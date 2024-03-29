﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Queries.Core.Parts.Columns;

/// <summary>
/// A list of values to use in <see cref="Clauses.ClauseOperator.In"/> condition
/// </summary>
public abstract class MultipleValues<T> : ColumnBase, IEquatable<MultipleValues<T>>, IEnumerable<T>
{
    /// <summary>
    /// Values
    /// </summary>
    public IEnumerable<T> Values { get; }

    /// <summary>
    /// Builds a new <see cref="MultipleValues{T}"/> instance.
    /// </summary>
    /// <param name="first">First value</param>
    /// <param name="others">Other values</param>
    protected MultipleValues(T first, params T[] others) => Values = new[] { first }.Concat(others.Where(val => val is not null));

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as MultipleValues<T>);

    ///<inheritdoc/>
    public bool Equals(MultipleValues<T> other) => other is not null && Values.SequenceEqual(other.Values);

    ///<inheritdoc/>
    public override int GetHashCode() => 1291433875 + Values.GetHashCode();

    ///<inheritdoc/>
    public override string ToString() => $"[{string.Join(",", Values)}]";

    ///<inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Values.GetEnumerator();

    ///<inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => Values.GetEnumerator();
}
