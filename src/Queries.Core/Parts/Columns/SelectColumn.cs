using Queries.Core.Builders;

using System;

namespace Queries.Core.Parts.Columns;

/// <summary>
/// a <see cref="Builders.SelectQuery"/> that can be used as 
/// <see cref="IColumn"/> in a <see cref="Builders.SelectQuery"/>.
/// </summary>
public class SelectColumn : IAliasable<SelectColumn>, IColumn, IEquatable<SelectColumn>
{
    private string _alias;

    /// <summary>
    /// Builds a new 
    /// </summary>
    /// <param name="select"></param>
    public SelectColumn(SelectQuery select) => SelectQuery = select;

    /// <summary>
    /// Alias of the column
    /// </summary>
    public string Alias => _alias;

    ///<inheritdoc/>
    public SelectColumn As(string alias)
    {
        _alias = alias;

        return this;
    }

    /// <summary>
    /// The wrapped query
    /// </summary>
    public SelectQuery SelectQuery { get; }

    /// <summary>
    /// Generates a unique identifier
    /// </summary>
    /// <returns></returns>
    public static UniqueIdentifierValue UUID() => new();

    /// <summary>
    /// Performs a deep copy of the current instance.
    /// </summary>
    /// <returns><see cref="SelectColumn"/> that is a deeo copy of the current instance.</returns>
    public IColumn Clone() => new SelectColumn(SelectQuery.Clone()).As(_alias);

    ///<inheritdoc/>
    public static implicit operator SelectColumn(SelectQuery select) => new(select);

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as SelectColumn);

    ///<inheritdoc/>
    public override int GetHashCode() => (SelectQuery, Alias).GetHashCode();

    ///<inheritdoc/>
    public bool Equals(SelectColumn other) => other != null && (SelectQuery, Alias).Equals((other.SelectQuery, other.Alias));

    ///<inheritdoc/>
    public override string ToString() => this.Jsonify();
}