using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;
using System;
using System.Linq;

namespace Queries.Core.Builders.Fluent
{
    /// <summary>
    /// Helper class start to building <see cref="IQuery"/> instances.
    /// </summary>
    public static class QueryBuilder
    {
        #region Columns conversions shortcuts

        /// <summary>
        /// Applies <see cref="LengthFunction"/> to <paramref name="column"/>.
        /// </summary>
        /// <param name="column">column onto which the function will be applied.</param>
        /// <returns><see cref="LengthFunction"/></returns>
        public static LengthFunction Length(IColumn column) => new LengthFunction(column);

        /// <summary>
        /// Concatenates <paramref name="columns"/>.
        /// </summary>
        /// <param name="columns">Columns to concatenate</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">if either <paramref name="first"/> or <paramref name="second"/> is <c>null</c> </exception>
        public static ConcatFunction Concat(IColumn first, IColumn second, params IColumn[] columns) => new ConcatFunction(first, second, columns);

        /// <summary>
        /// Applies <see cref="NullFunction"/> to <paramref name="column"/>.
        /// </summary>
        /// <param name="column">Column onto which aoply <see cref="NullFunction"/>.</param>
        /// <param name="fallBackValue"></param>
        /// <param name="otherFallbackValues"></param>
        /// <param name="defaultValue">Result value to use if <paramref name="column"/>'s value is <c>null</c></param>
        /// <returns></returns>
        public static NullFunction Null(FieldColumn column, ColumnBase fallBackValue, params ColumnBase[] otherFallbackValues) => new NullFunction(column, fallBackValue, otherFallbackValues);

        /// <summary>
        /// Applies <see cref="CountFunction"/> to <paramref name="column"/>.
        /// </summary>
        /// <param name="column">Column onto which apply <see cref="CountFunction"/>.</param>
        /// <returns></returns>
        public static CountFunction Count(FieldColumn column) => new CountFunction(column);

        /// <summary>
        /// Applies <see cref="MinFunction"/> to <paramref name="column"/>.
        /// </summary>
        /// <param name="column">Column onto which aoply <see cref="MinFunction"/>.</param>
        /// <returns></returns>
        public static MinFunction Min(IColumn column) => new MinFunction(column);

        /// <summary>
        /// Applies <see cref="MinFunction"/> to <paramref name="column"/>.
        /// </summary>
        /// <param name="columnName">Name of the column onto which aoply <see cref="MinFunction"/>.</param>
        /// <returns></returns>
        public static MinFunction Min(string columnName) => new MinFunction(columnName);

        /// <summary>
        /// Applies <see cref="AvgFunction"/> to <paramref name="column"/>.
        /// </summary>
        /// <param name="column">Column onto which aoply <see cref="AvgFunction"/>.</param>
        /// <returns></returns>
        public static AvgFunction Avg(IColumn column) => new AvgFunction(column);

        /// <summary>
        /// Applies the "Avg" function to the specified column
        /// </summary>
        /// <param name="columnName">Name of the column to applied the AVG column</param>
        /// <returns></returns>
        public static AvgFunction Avg(string columnName) => new AvgFunction(columnName);

        /// <summary>
        /// Creates a new <see cref="SubstringFunction"/>
        /// </summary>
        /// <param name="column">Column to which the function will be applied</param>
        /// <param name="start">position where the substring will start</param>
        /// <param name="length">position where the substring will end</param>
        /// <returns><see cref="SubstringFunction"/></returns>
        public static SubstringFunction Substring(IColumn column, int start, int? length = null) => new SubstringFunction(column, start, length);

        /// <summary>
        /// Applies the "UPPER" function to the specified column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static UpperFunction Upper(IColumn column) => new UpperFunction(column);

        /// <summary>
        /// Wraps a <see cref="IColumn"/> into <see cref="MaxFunction"/> function
        /// </summary>
        /// <param name="column"></param>
        /// <returns><see cref="MaxFunction"/></returns>
        public static MaxFunction Max(IColumn column) => new MaxFunction(column);

        #endregion

        /// <summary>
        /// Creates a <see cref="DeleteQuery"/>
        /// </summary>
        /// <param name="tableName">Name of the table to be deleted</param>
        /// <returns></returns>
        public static DeleteQuery Delete(string tableName) => new DeleteQuery(tableName);

        /// <summary>
        /// Creates a <see cref="CreateViewQuery"/> object suitable to build <a href="http://www.w3schools.com/sql/sql_views.asp">CREATE VIEW</a> query
        /// </summary>
        /// <param name="viewName">Name of the view the query will be generated for</param>
        /// <returns></returns>
        public static CreateViewQuery CreateView(string viewName) => new CreateViewQuery(viewName);

        /// <summary>
        /// Creates a <see cref="UpdateQuery"/> object suitable to build <a href="http://www.w3schools.com/sql/sql_update.asp">UPDATE</a> query
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns><see cref="UpdateQuery"/></returns>
        public static UpdateQuery Update(string tableName) => new UpdateQuery(tableName);

        /// <summary>
        /// Creates a <see cref="SelectIntoQuery"/> object suitable to build <a href="http://www.w3schools.com/sql/sql_select_into.asp">SELECT INTO</a> query
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static SelectIntoQuery SelectInto(string tableName) => new SelectIntoQuery(tableName);

        /// <summary>
        /// Creates a <see cref="SelectQuery"/> object suitable to build <a href="http://www.w3schools.com/sql/sql_select.asp">SELECT</a> query
        /// </summary>
        /// <param name="columnNames">The column names.</param>
        /// <returns><see cref="SelectQuery"/></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">if <see cref="columnNames"/> is empty or contains only <c>null</c> elements.</exception>
        public static SelectQuery Select(params string[] columnNames) => new SelectQuery(columnNames);

        /// <summary>
        /// Creates a <see cref="SelectQuery"/> object suitable to build <a href="http://www.w3schools.com/sql/sql_select.asp">SELECT</a> query
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns><see cref="SelectQuery"/></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">if <see cref="columns"/> is empty or contains only <c>null</c> elements.</exception>
        public static SelectQuery Select(params IColumn[] columns) => new SelectQuery(columns);

        /// <summary>
        /// Creates a <see cref="TruncateQuery"/> object suitable to build <a href="http://www.w3schools.com/sql/sql_truncate.asp">SELECT</a> query
        /// </summary>
        /// <param name="tableName">Name of the table the <see cref="TruncateQuery"/> is built for</param>
        /// <returns><see cref="TruncateQuery"/></returns>
        /// <exception cref="System.ArgumentNullException">if <paramref name="tableName"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">if <paramref name="tableName"/> is empty or whitespace.</exception>
        public static TruncateQuery Truncate(string tableName) => new TruncateQuery(tableName);

        /// <summary>
        /// Creates a <see cref="InsertIntoQuery"/> object suitable to build <a href="http://www.w3schools.com/sql/sql_insert.asp">INSERT INTO</a> or <a href="http://www.w3schools.com/sql/sql_insert_into_select.asp">INSERT INTO SELECT</a> query
        /// </summary>
        /// <param name="tableName">Name of the table the <see cref="InsertIntoQuery"/> is built for</param>
        /// <returns><see cref="InsertIntoQuery"/></returns>
        public static InsertIntoQuery InsertInto(string tableName) => new InsertIntoQuery(tableName);

        /// <summary>
        /// Creates a <see cref="DeclareVariableQuery"/> object suitable to create a variable in a script
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public static DeclareVariableQuery Declare(string variableName) => new DeclareVariableQuery(variableName);

        /// <summary>
        /// Creates a <see cref="CasesColumn"/>
        /// </summary>
        /// <param name="first"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"> if <paramref name="whenExpressions"/> is <c>null</c></exception>
        public static CasesColumn Cases(WhenExpression first, params WhenExpression[] others)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (others == null)
            {
                throw new ArgumentNullException(nameof(others));
            }

            return new CasesColumn(new[] { first }.Union(others.Where(x => x != null)));
        }

        /// <summary>
        /// Creates a new <see cref="WhenExpression"/> instance.
        /// </summary>
        /// <param name="criterion"></param>
        /// <param name="then">Value to use when <paramref name="criterion"/> is not satisfied.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">either <paramref name="criterion"/> or <paramref name="then"/> is <c>null</c></exception>
        public static WhenExpression When(WhereClause criterion, ColumnBase then) => new WhenExpression(criterion, then);
    }
}