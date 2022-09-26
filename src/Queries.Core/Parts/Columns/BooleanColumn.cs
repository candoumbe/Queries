namespace Queries.Core.Parts.Columns;

/// <summary>
/// Column that contains a <see cref="bool"/> value.
/// </summary>
public class BooleanColumn : Literal
{
    /// <summary>
    /// Builds a new <see cref="BooleanColumn"/> instance.
    /// </summary>
    /// <param name="value">The value hold by the column</param>
    public BooleanColumn(bool value) : base(value)
    { }
}