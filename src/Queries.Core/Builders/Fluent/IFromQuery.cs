using Queries.Core.Parts;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Sorting;

namespace Queries.Core.Builders.Fluent;

/// <summary>
/// Defines additional operations that can be performed on <see cref="IFromQuery{T}"/>
/// </summary>
/// <typeparam name="T">Type of the query that is being built.</typeparam>
public interface IFromQuery<T> : IUnionQuery<T>
{
    /// <summary>
    /// Performs a INNER JOIN
    /// </summary>
    /// <param name="table"></param>
    /// <param name="on">criteria used to perform the join</param>
    /// <returns></returns>
    IJoinQuery<T> InnerJoin(Table table, IWhereClause on);

    /// <summary>
    /// Performs a LEFT OUTER JOIN
    /// </summary>
    /// <param name="table"></param>
    /// <param name="on">criteria used to perform the join</param>
    /// <returns><see cref="IJoinQuery{T}"/> to perform additinal operations.</returns>
    IJoinQuery<T> LeftOuterJoin(Table table, IWhereClause on);

    /// <summary>
    /// Performs a RIGHT OUTER JOIN
    /// </summary>
    /// <param name="table"></param>
    /// <param name="on">criteria used to perform the join</param>
    /// <returns><see cref="IJoinQuery{T}"/> to perform additinal operations.</returns>
    IJoinQuery<T> RightOuterJoin(Table table, IWhereClause on);

    /// <summary>
    /// Adds <paramref name="orders"/> statements
    /// </summary>
    /// <param name="orders">how element should be sorted</param>
    /// <returns>The current instance for further processing</returns>
    IOrderQuery<T> OrderBy(params IOrder[] orders);

    /// <summary>
    /// Adds <paramref name="clause"/> to the instance under construction
    /// </summary>
    /// <param name="clause">how element should be sorted</param>
    /// <returns>The current instance for further processing</returns>
    IWhereQuery<T> Where(IWhereClause clause);

    /// <summary>
    /// Adds a <see cref="IWhereClause"/> clause based on the specified <paramref name="column"/>, <paramref name="operator"/> and <paramref name="constraint"/>.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="operator"></param>
    /// <param name="constraint"></param>
    /// <returns>The current instance for further processing</returns>
    IWhereQuery<T> Where(IColumn column, ClauseOperator @operator, IColumn constraint);

    /// <summary>
    /// Adds a <see cref="IWhereClause"/> clause based on the specified <paramref name="column"/>, <paramref name="operator"/> and <paramref name="constraint"/>.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="operator"></param>
    /// <param name="constraint"></param>
    /// <returns>The current instance for further processing</returns>
    IWhereQuery<T> Where(IColumn column, ClauseOperator @operator, string constraint);
}