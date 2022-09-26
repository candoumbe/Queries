using System;
using Queries.Core.Parts.Columns;
using Queries.Core.Attributes;

namespace Queries.Core.Parts.Functions;

/// <summary>
/// "UPPER" function.
/// </summary>
[Function]
public class UpperFunction : IColumn, IAliasable<UpperFunction>
{
    /// <summary>
    /// Column onto wich the function applies
    /// </summary>
    public IColumn Column { get; }

    /// <summary>
    /// Alias of the result of the function
    /// </summary>
    public string Alias { get; private set; }

    /// <summary>
    /// Builds a new <see cref="UpperFunction"/> instance
    /// </summary>
    /// <param name="column">Column the function will be applied on</param>
    /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <code>null</code></exception>
    public UpperFunction(IColumn column)
    {
        Column = column ?? throw new ArgumentNullException(nameof(column));
    }

    /// <summary>
    /// Builds a new <see cref="UpperFunction"/> instance.
    /// </summary>
    /// <param name="value">the value the function will be applied on</param>
    /// <exception cref="ArgumentNullException">if <paramref name="value"/> is <code>null</code></exception>
    public UpperFunction(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        Column = value.Literal();
    }

    ///<inheritdoc/>
    public UpperFunction As(string alias)
    {
        Alias = alias;

        return this;
    }

    /// <summary>
    /// Performs a deep copy of the current instance.
    /// </summary>
    /// <returns><see cref="UpperFunction"/></returns>
    public IColumn Clone() => new UpperFunction(Column.Clone());
}