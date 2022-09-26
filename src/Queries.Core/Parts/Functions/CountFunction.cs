using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Functions;

/// <summary>
/// "COUNT" function.
/// </summary>
public class CountFunction : AggregateFunction
{
    /// <summary>
    /// Builds a new <see cref="CountFunction"/> instance
    /// </summary>
    /// <param name="column">column onto which the function will bne applied</param>
    /// <exception cref="System.ArgumentNullException">if <paramref name="column"/> is <see langword="null" /></exception>
    public CountFunction(FieldColumn column)
        : base(AggregateType.Count, column)
    { }


    /// <summary>
    /// Performs a deep copy of the current instance.
    /// </summary>
    /// <returns><see cref="CountFunction"/></returns>
    public override IColumn Clone() => new CountFunction((FieldColumn)Column.Clone());
}