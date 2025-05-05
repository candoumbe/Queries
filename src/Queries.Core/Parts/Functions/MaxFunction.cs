using System;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Functions;

/// <summary>
/// "MAX" function
/// </summary>
public class MaxFunction : AggregateFunction
{
    /// <summary>
    /// Builds a new <see cref="MaxFunction"/> instance.
    /// </summary>
    /// <param name="column">The column the function will be applied onto</param>
    /// <exception cref="System.ArgumentNullException">if <paramref name="column"/> is <see langword="null" />.</exception>
    public MaxFunction(IColumn column) : base(AggregateType.Max, column)
    { }

    /// <summary>
    /// Builds a new <see cref="MaxFunction"/> instance.
    /// </summary>
    /// <param name="columnName">name of the column onto which the function will be applied.</param>
    /// <exception cref="System.ArgumentNullException">if <paramref name="columnName"/> is <see langword="null" /></exception>
    public MaxFunction(string columnName) : this(columnName?.Field())
    {
    }

    /// <summary>
    /// Performs a deep copy of the current instance.
    /// </summary>
    /// <returns><see cref="MaxFunction"/></returns>
    public override IColumn Clone() => new MaxFunction(Column.Clone());

#if NET8_0_OR_GREATER
    /// <inheritdoc />
    public override MaxFunction As(string alias)
    {
        base.As(alias);
        return this;
    }
#endif
}