namespace Queries.Renderers.Postgres.Builders.Fluent;

/// <summary>
/// Fluent builder for <see cref="ReturnQuery"/>
/// </summary>
public static class ReturnBuilder
{
    /// <summary>
    /// Builds a <see cref="ReturnQuery"/> which returns <paramref name="returnValue"/>.
    /// </summary>
    /// <param name="returnValue">The "value" to return</param>
    /// 
    /// <returns></returns>
    public static ReturnQuery Return(ColumnBase returnValue)
    {
        if (returnValue == null)
        {
            throw new ArgumentNullException(nameof(returnValue));
        }
        return new ReturnQuery(Option.Some<ColumnBase, SelectQuery>(returnValue));
    }

    /// <summary>
    /// Builds a <see cref="ReturnQuery"/> which returns <paramref name="returnValue"/> for <paramref name="returnValue"/>.
    /// </summary>
    /// <param name="returnValue">The "value" to return</param>
    /// <returns>a <see cref="ReturnQuery"/></returns>
    public static ReturnQuery Return(SelectQuery returnValue)
    {
        if (returnValue == null)
        {
            throw new ArgumentNullException(nameof(returnValue));
        }
        return new ReturnQuery(Option.None<ColumnBase, SelectQuery>(returnValue));
    }

    /// <summary>
    /// Builds a <see cref="ReturnQuery"/> which returns.
    /// </summary>
    /// <returns>a <see cref="ReturnQuery"/></returns>
    public static ReturnQuery Return() => new(Option.None<ColumnBase, SelectQuery>(default));
}
