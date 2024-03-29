#if !SYSTEM_TEXT_JSON
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; 
#endif
using Queries.Core.Attributes;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Queries.Core.Parts.Functions;

/// <summary>
/// Concat two or more columns.
/// </summary>
[Function]
#if !SYSTEM_TEXT_JSON
		[JsonObject(ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
	#endif
public class ConcatFunction : IAliasable<ConcatFunction>, IColumn, IEquatable<ConcatFunction>
{
    /// <summary>
    /// List of columns to concat
    /// </summary>
    public IEnumerable<IColumn> Columns { get; }

    /// <summary>
    /// Builds a new <see cref="ConcatFunction"/> instance.
    /// </summary>
    /// <param name="first">First column to concatenate</param>
    /// <param name="second">Second column to concatenate</param>
    /// <param name="columns">Additional columns to concatenate.</param>
    /// <exception cref="ArgumentNullException">if either <paramref name="first"/> or <paramref name="second"/> is <see langword="null" />.</exception>
    public ConcatFunction(IColumn first, IColumn second, params IColumn[] columns)
    {
        if (first == null)
        {
            throw new ArgumentNullException(nameof(first));
        }
        if (second == null)
        {
            throw new ArgumentNullException(nameof(second));
        }

        IEnumerable<IColumn> localColumns = (columns ?? Enumerable.Empty<IColumn>())
            .Where(x => x != null)
            .ToArray();

        Columns = new[] { first, second }.Union(localColumns);
    }

    private string _alias;

    /// <summary>
    /// Alias of the result of the result of the function.
    /// </summary>
    public string Alias => _alias;

    /// <summary>
    /// Gives the result of the concat an alias
    /// </summary>
    /// <param name="alias">The new alias</param>
    /// <returns>The current instance</returns>
    public ConcatFunction As(string alias)
    {
        _alias = alias;

        return this;
    }

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as ConcatFunction);

    ///<inheritdoc/>
    public bool Equals(ConcatFunction other) => other is not null
        && Columns.SequenceEqual(other.Columns) && Alias == other.Alias;

    ///<inheritdoc/>
    public override int GetHashCode()
    {
        int hashCode = -1367283405;
        hashCode = (hashCode * -1521134295) + EqualityComparer<IEnumerable<IColumn>>.Default.GetHashCode(Columns);
        return (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Alias);
    }

    ///<inheritdoc/>
    public override string ToString()
    {
        var props = new
        {
            Type = nameof(ConcatFunction),
            Columns,
            Alias
        };

        return props.Jsonify();
    }

    /// <summary>
    /// Performs a deep copy of the current instance.
    /// </summary>
    /// <returns><see cref="ConcatFunction"/></returns>
    public IColumn Clone() => new ConcatFunction(Columns.ElementAt(0), Columns.ElementAt(1), Columns.Skip(2).Select(x => x.Clone()).ToArray());
}