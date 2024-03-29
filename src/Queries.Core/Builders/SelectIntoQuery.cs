using Queries.Core.Builders.Fluent;
using Queries.Core.Parts;

using System;

namespace Queries.Core.Builders;

/// <summary>
/// A query to create a new collection of data from a <see cref="SelectQuery"/>.
/// </summary>
public class SelectIntoQuery : SelectQueryBase, IBuild<SelectIntoQuery>
{
    /// <summary>
    /// Where to insert data.
    /// </summary>
    public Table Destination { get; set; }

    /// <summary>
    /// where to gather data from
    /// </summary>
    public ITable Source { get; set; }

    /// <summary>
    /// Builds a new <see cref="SelectIntoQuery"/> instance.
    /// </summary>
    /// <param name="table"></param>
    public SelectIntoQuery(string table) : this(table.Table())
    { }

    /// <summary>
    /// Builds a new <see cref="SelectIntoQuery"/> instance.
    /// </summary>
    /// <param name="table"></param>
    /// <exception cref="ArgumentNullException"><paramref name="table"/> is <see langword="null"/></exception>
    public SelectIntoQuery(Table table)
    {
        Destination = table ?? throw new ArgumentNullException(nameof(table));
    }

    /// <summary>
    /// Defines <see cref="Source"/>
    /// </summary>
    /// <param name="source"></param>
    /// <returns><see cref="IBuild{SelectIntoQuery}"/> for further processing.</returns>
    public IBuild<SelectIntoQuery> From(ITable source)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        return this;
    }

    ///<inheritdoc/>
    public SelectIntoQuery Build() => this;
}