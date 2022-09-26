#if !SYSTEM_TEXT_JSON
using Newtonsoft.Json; 
#endif

using Queries.Core.Attributes;
using Queries.Core.Parts.Columns;

using System;

namespace Queries.Core.Parts.Functions;

/// <summary>
/// Base class for all aggregate function.
/// </summary>
[Function]
#if !SYSTEM_TEXT_JSON
[JsonObject] 
#endif
public abstract class AggregateFunction : IAliasable<AggregateFunction>, IEquatable<AggregateFunction>, IColumn
{
    /// <summary>
    /// The type of aggregate the current instance represents
    /// </summary>
    public AggregateType Type { get; }

    /// <summary>
    /// The column onto which the current function will be applied
    /// </summary>
    public IColumn Column { get; }

    /// <summary>
    /// Builds a new <see cref="AggregateFunction"/> instance.
    /// </summary>
    /// <param name="aggregate">The aggregate function to create</param>
    /// <param name="column">The cokumn onto which the aggregate will be applied</param>
    /// <exception cref="ArgumentNullException">if <paramref name="column"/> is null.</exception>
    protected AggregateFunction(AggregateType aggregate, IColumn column)
    {
        Type = aggregate;
        Column = column ?? throw new ArgumentNullException(nameof(column));
    }

    private string _alias;

    /// <summary>
    /// The alias associated with the column
    /// </summary>
    public string Alias => _alias;

    ///<inheritdoc/>
    public AggregateFunction As(string alias)
    {
        _alias = alias;

        return this;
    }

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as AggregateFunction);

    ///<inheritdoc/>
    public bool Equals(AggregateFunction other) => (Column, Alias).Equals((other?.Column, other?.Alias));

    ///<inheritdoc/>
    public override int GetHashCode() => (Column, Alias).GetHashCode();

    ///<inheritdoc/>
    public override string ToString() => this.Jsonify();

    ///<inheritdoc/>
    public abstract IColumn Clone();
}
