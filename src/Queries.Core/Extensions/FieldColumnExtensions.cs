using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;

using System;


namespace Queries.Core.Parts.Columns;

/// <summary>
/// Extension methods for <see cref="FieldColumn"/> type
/// </summary>
public static class FieldColumnExtensions
{
    /// <summary>
    /// Builds an <see cref="UpdateFieldValue"/>
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"><paramref name="destination"/> is <see langword="null"/></exception>
    public static UpdateFieldValue UpdateValueTo(this FieldColumn destination, ColumnBase source)
    {
        return destination == null ? throw new ArgumentNullException(nameof(destination)) : new UpdateFieldValue(destination, source);
    }

    /// <summary>
    /// Creates a <see cref="InsertedValue"/>.
    /// </summary>
    /// <param name="columnName"></param>
    /// <param name="column"></param>
    /// <returns>a <see cref="InsertedValue"/></returns>
    public static InsertedValue InsertValue(this FieldColumn columnName, IColumn column)
        => new(columnName, column);

    /// <summary>
    /// Creates a <see cref="WhereClause"/> which states <paramref name="column"/>'s value is <see langword="null" />.
    /// </summary>
    /// <param name="column">Column to apply the clause onto</param>
    /// <returns></returns>
    public static WhereClause IsNull(this FieldColumn column) => new(column, ClauseOperator.IsNull);

    /// <summary>
    /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'s value is not <see langword="null" />.
    /// </summary>
    /// <param name="column">Column to apply the clause onto</param>
    /// <returns></returns>
    public static WhereClause IsNotNull(this FieldColumn column) => new(column, ClauseOperator.IsNotNull);

    /// <summary>
    /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'value is <c> &lt; </c> <paramref name="constraint"/>'s value.
    /// </summary>
    /// <param name="column">Column to apply the clause onto</param>
    /// <param name="constraint"></param>
    /// <returns></returns>
    public static WhereClause LessThan(this IColumn column, ColumnBase constraint) => new(column, ClauseOperator.LessThan, constraint);

    /// <summary>
    /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'value is <c>&lt;</c> or equal to <paramref name="constraint"/>'s value.
    /// </summary>
    /// <param name="column">Column to apply the clause onto</param>
    /// <param name="constraint"></param>
    /// <returns></returns>
    public static WhereClause LessThanOrEqualTo(this FieldColumn column, ColumnBase constraint) => new(column, ClauseOperator.LessThanOrEqualTo, constraint);

    /// <summary>
    /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'value is <c> &gt; </c> <paramref name="constraint"/>'s value.
    /// </summary>
    /// <param name="column">Column to apply the clause onto</param>
    /// <param name="constraint"></param>
    /// <returns></returns>
    public static WhereClause GreaterThan(this FieldColumn column, ColumnBase constraint) => new(column, ClauseOperator.GreaterThan, constraint);

    /// <summary>
    /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'value is <c></c> or equal to <paramref name="constraint"/>'s value.
    /// </summary>
    /// <param name="column">Column to apply the clause onto</param>
    /// <param name="constraint"></param>
    /// <returns><see cref="WhereClause"/></returns>
    public static WhereClause GreaterThanOrEqualTo(this FieldColumn column, ColumnBase constraint) => new(column, ClauseOperator.GreaterThanOrEqualTo, constraint);

    /// <summary>
    /// Creates a <see cref="WhereClause"/> equivalent to <code>column = constraint</code>
    /// </summary>
    /// <param name="column"></param>
    /// <param name="constraint"></param>
    /// <returns></returns>
    public static WhereClause EqualTo(this FieldColumn column, ColumnBase constraint) => new(column, ClauseOperator.EqualTo, constraint);

    /// <summary>
    /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'s value is one the value
    /// </summary>
    /// <param name="column">column to apply the clause onto</param>
    /// <param name="first"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static WhereClause In(this FieldColumn column, string first, params string[] values) => new(column, ClauseOperator.In, new StringValues(first, values));

    /// <summary>
    /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'s value is in the <see cref="SelectColumn"/>
    /// </summary>
    /// <param name="column">column to apply the clause onto</param>
    /// <param name="select"></param>
    /// <returns></returns>
    public static WhereClause In(this FieldColumn column, SelectQuery select) => new(column, ClauseOperator.In, select);

    /// <summary>
    /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'s value is not one of <paramref name="first"/> or
    /// <paramref name="values"/>.
    /// </summary>
    /// <param name="column">column to apply the clause onto</param>
    /// <param name="first"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static WhereClause NotIn(this FieldColumn column, string first, params string[] values) => new(column, ClauseOperator.NotIn, new StringValues(first, values));

    /// <summary>
    /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'s value is in the <see cref="SelectColumn"/>
    /// </summary>
    /// <param name="column">column to apply the clause onto</param>
    /// <param name="select"></param>
    /// <returns></returns>
    public static WhereClause NotIn(this FieldColumn column, SelectQuery select) => new(column, ClauseOperator.NotIn, select);

    /// <summary>
    /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'s value is like <paramref name="variable"/>.
    /// </summary>
    /// <param name="column">column to apply the clause onto</param>
    /// <param name="variable"></param>
    /// <returns></returns>
    public static WhereClause Like(this FieldColumn column, IColumn variable) => new(column, ClauseOperator.Like, variable);

    /// <summary>
    /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'s value is like <paramref name="variable"/>.
    /// </summary>
    /// <param name="column">column to apply the clause onto</param>
    /// <param name="variable"></param>
    /// <returns></returns>
    public static WhereClause Like(this FieldColumn column, string variable) => new(column, ClauseOperator.Like, variable.Literal());
}