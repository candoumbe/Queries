namespace Queries.Core.Parts.Columns;

/// <summary>
/// Extensions method
/// </summary>
public static class FieldColumnExtensions
{
    /// <summary>
    /// Wraps the specified <paramref name="column"/> so that in can queried as JSON column.
    /// </summary>
    /// <param name="column">The column to wrap</param>
    /// <param name="path"></param>
    /// <returns>a <see cref="JsonFieldColumn"/></returns>
    public static JsonFieldColumn Json(this FieldColumn column, string path = null) => new(column, path);
}
