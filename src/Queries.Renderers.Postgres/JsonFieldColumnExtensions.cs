namespace Queries.Renderers.Postgres;

/// <summary>
/// Extension method for <see cref="JsonFieldColumn"/> types.
/// </summary>
public static class JsonFieldColumnExtensions
{
    /// <summary>
    /// Builds a <see cref="WhereClause"/> instance.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="constraint"></param>
    /// <returns></returns>
    public static WhereClause EqualTo(this JsonFieldColumn column, ColumnBase constraint) => new(column, ClauseOperator.EqualTo, constraint);
}
