using System;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;

namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// Extension methods for <see cref="FieldColumn"/> type
    /// </summary>
    public static class FieldColumnExtensions
    {
        public static UpdateFieldValue UpdateValueTo(this FieldColumn destination, ColumnBase source)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            return new UpdateFieldValue(destination, source);
        }

        public static InsertedValue InsertValue(this FieldColumn columnName, IColumn column)
            => new InsertedValue(columnName, column);

        /// <summary>
        /// Creates a <see cref="WhereClause"/> which states <paramref name="column"/>'s value is <c>null</c>.
        /// </summary>
        /// <param name="column">Column to apply the clause onto</param>
        /// <returns></returns>
        public static WhereClause IsNull(this FieldColumn column) => new WhereClause(column, ClauseOperator.IsNull);

        /// <summary>
        /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'s value is not <c>null</c>.
        /// </summary>
        /// <param name="column">Column to apply the clause onto</param>
        /// <returns></returns>
        public static WhereClause IsNotNull(this FieldColumn column) => new WhereClause(column, ClauseOperator.IsNotNull);

        /// <summary>
        /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'value is <c> &lt; </c> <paramref name="constraint"/>'s value.
        /// </summary>
        /// <param name="column">Column to apply the clause onto</param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static WhereClause LessThan(this IColumn column, ColumnBase constraint) => new WhereClause(column, ClauseOperator.LessThan, constraint);

        /// <summary>
        /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'value is <c>&lt;</c> or equal to <paramref name="constraint"/>'s value.
        /// </summary>
        /// <param name="column">Column to apply the clause onto</param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static WhereClause LessThanOrEqualTo(this FieldColumn column, ColumnBase constraint) => new WhereClause(column, ClauseOperator.LessThanOrEqualTo, constraint);

        /// <summary>
        /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'value is <c> &gt; </c> <paramref name="constraint"/>'s value.
        /// </summary>
        /// <param name="column">Column to apply the clause onto</param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static WhereClause GreaterThan(this FieldColumn column, ColumnBase constraint) => new WhereClause(column, ClauseOperator.GreaterThan, constraint);

        /// <summary>
        /// Creates a <see cref="WhereClause"/> that states<c><paramref name="column"/>'value is <c> &gt; </c> or equal to <paramref name="constraint"/>'s value.
        /// </summary>
        /// <param name="column">Column to apply the clause onto</param>
        /// <returns></returns>
        public static WhereClause GreaterThanOrEqualTo(this FieldColumn column, ColumnBase constraint) => new WhereClause(column, ClauseOperator.GreaterThanOrEqualTo, constraint);

        /// <summary>
        /// Creates a <see cref="WhereClause"/> equivalent to <code>column = constraint</code>
        /// </summary>
        /// <param name="column"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static WhereClause EqualTo(this FieldColumn column, ColumnBase constraint) => new WhereClause(column, ClauseOperator.EqualTo, constraint);

        /// <summary>
        /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'s value is one the 
        /// </summary>
        /// <param name="column">column to apply the clause onto</param>
        /// <param name="first"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static WhereClause In(this FieldColumn column, string first, params string[] values) => new WhereClause(column, ClauseOperator.In, new StringValues(first, values));

        /// <summary>
        /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'s value is in the <see cref="SelectColumn"/>
        /// </summary>
        /// <param name="column">column to apply the clause onto</param>
        /// <param name="select"></param>
        /// <returns></returns>
        public static WhereClause In(this FieldColumn column, SelectQuery select) => new WhereClause(column, ClauseOperator.In, select);

        /// <summary>
        /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'s value is not one of <paramref name="first"/> or
        /// <paramref name="values"/>.
        /// </summary>
        /// <param name="column">column to apply the clause onto</param>
        /// <param name="first"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static WhereClause NotIn(this FieldColumn column, string first, params string[] values) => new WhereClause(column, ClauseOperator.NotIn, new StringValues(first, values));

        /// <summary>
        /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'s value is in the <see cref="SelectColumn"/>
        /// </summary>
        /// <param name="column">column to apply the clause onto</param>
        /// <param name="select"></param>
        /// <returns></returns>
        public static WhereClause NotIn(this FieldColumn column, SelectQuery select) => new WhereClause(column, ClauseOperator.NotIn, select);

        /// <summary>
        /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'s value is like <paramref name="variable"/>.
        /// </summary>
        /// <param name="column">column to apply the clause onto</param>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static WhereClause Like(this FieldColumn column, IColumn variable) => new WhereClause(column, ClauseOperator.Like, variable);

        /// <summary>
        /// Creates a <see cref="WhereClause"/> that states <paramref name="column"/>'s value is like <paramref name="variable"/>.
        /// </summary>
        /// <param name="column">column to apply the clause onto</param>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static WhereClause Like(this FieldColumn column, string variable) => new WhereClause(column, ClauseOperator.Like, variable.Literal());
    }
}