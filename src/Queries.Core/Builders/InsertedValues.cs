using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Queries.Core.Parts.Columns;
using System;
using System.Linq;

namespace Queries.Core.Builders;

/// <summary>
/// A collection of values to insert. This class is intended to be used in conjuction with <see cref="InsertIntoQuery"/>
/// </summary>
public class InsertedValues : IEnumerable<InsertedValue>, IInsertable, IEquatable<InsertedValues>
{
    private IReadOnlyCollection<InsertedValue> Columns => new ReadOnlyCollection<InsertedValue>(_columns);

    private readonly IList<InsertedValue> _columns;

    /// <summary>
    /// Builds a new <see cref="InsertedValues"/> instance.
    /// </summary>
    public InsertedValues() => _columns = new List<InsertedValue>();

    /// <summary>
    /// Adds <paramref name="column"/>
    /// </summary>
    /// <param name="column"></param>
    public void Add(InsertedValue column) => _columns.Add(column);

    ///<inheritdoc/>
    public IEnumerator<InsertedValue> GetEnumerator() => Columns.GetEnumerator();

    ///<inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Columns).GetEnumerator();

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as InsertedValues);

    ///<inheritdoc/>
    public bool Equals(InsertedValues other) => other != null && Columns.SequenceEqual(other.Columns);

    ///<inheritdoc/>
    public override int GetHashCode() => -1952516548 + EqualityComparer<IReadOnlyCollection<InsertedValue>>.Default.GetHashCode(Columns);
}