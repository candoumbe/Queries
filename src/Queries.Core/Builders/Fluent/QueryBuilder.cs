namespace Queries.Core.Builders.Fluent;

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
    public static LengthFunction Length(IColumn column) => new(column);
    /// <summary>
    /// Applies <see cref="LengthFunction"/> to <paramref name="column"/>.
    /// </summary>
    /// <param name="column">column onto which the function will be applied.</param>
    /// <returns><see cref="LengthFunction"/></returns>
    public static LengthFunction Length(IColumn column) => new(column);

    /// <summary>
    /// Concatenates <paramref name="columns"/>.
    /// </summary>
    /// <param name="first">The first <see cref="IColumn"/></param>
    /// <param name="second">The second<see cref="IColumn"/></param>
    /// <param name="columns">Columns to concatenate</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">if either <paramref name="first"/> or <paramref name="second"/> is <see langword="null" /> </exception>
    public static ConcatFunction Concat(IColumn first, IColumn second, params IColumn[] columns) => new(first, second, columns);

    /// <summary>
    /// Applies <see cref="NullFunction"/> to <paramref name="column"/>.
    /// </summary>
    /// <param name="column">Column onto which aoply <see cref="NullFunction"/>.</param>
    /// <param name="fallBackValue"></param>
    /// <param name="otherFallbackValues"></param>
    /// <returns>A <see cref="NullFunction"/> that will default to either <paramref name="fallBackValue"/> or one of the <paramref name="otherFallbackValues"/></returns>
    public static NullFunction Null(FieldColumn column, ColumnBase fallBackValue, params ColumnBase[] otherFallbackValues) => new(column, fallBackValue, otherFallbackValues);

    /// <summary>
    /// Applies <see cref="CountFunction"/> to <paramref name="column"/>.
    /// </summary>
    /// <param name="column">Column onto which apply <see cref="CountFunction"/>.</param>
    /// <returns></returns>
    public static CountFunction Count(FieldColumn column) => new(column);

    /// <summary>
    /// Applies <see cref="MinFunction"/> to <paramref name="column"/>.
    /// </summary>
    /// <param name="column">Column onto which aoply <see cref="MinFunction"/>.</param>
    /// <returns></returns>
    public static MinFunction Min(IColumn column) => new(column);

    /// <summary>
    /// Applies <see cref="MinFunction"/> to <paramref name="columnName"/>.
    /// </summary>
    /// <param name="columnName">Name of the column onto which aoply <see cref="MinFunction"/>.</param>
    /// <returns></returns>
    public static MinFunction Min(string columnName) => new(columnName);

    /// <summary>
    /// Applies <see cref="AvgFunction"/> to <paramref name="column"/>.
    /// </summary>
    /// <param name="column">Column onto which aoply <see cref="AvgFunction"/>.</param>
    /// <returns></returns>
    public static AvgFunction Avg(IColumn column) => new(column);

    /// <summary>
    /// Applies the "Avg" function to the specified column
    /// </summary>
    /// <param name="columnName">Name of the column to applied the AVG column</param>
    /// <returns></returns>
    public static AvgFunction Avg(string columnName) => new(columnName);

    /// <summary>
    /// Creates a new <see cref="SubstringFunction"/>
    /// </summary>
    /// <param name="column">Column to which the function will be applied</param>
    /// <param name="start">position where the substring will start</param>
    /// <param name="length">position where the substring will end</param>
    /// <returns><see cref="SubstringFunction"/></returns>
    public static SubstringFunction Substring(IColumn column, int start, int? length = null) => new(column, start, length);

    /// <summary>
    /// Applies the "UPPER" function to the specified column
    /// </summary>
    /// <param name="column"></param>
    /// <returns></returns>
    public static UpperFunction Upper(IColumn column) => new(column);

    /// <summary>
    /// Wraps a <see cref="IColumn"/> into <see cref="MaxFunction"/> function
    /// </summary>
    /// <param name="column"></param>
    /// <returns><see cref="MaxFunction"/></returns>
    public static MaxFunction Max(IColumn column) => new(column);

    #endregion

    /// <summary>
    /// Creates a <see cref="DeleteQuery"/>
    /// </summary>
    /// <param name="tableName">Name of the table to be deleted</param>
    /// <returns></returns>
    public static DeleteQuery Delete(string tableName) => new(tableName);

    /// <summary>
    /// Creates a <see cref="CreateViewQuery"/> object suitable to build <a href="http://www.w3schools.com/sql/sql_views.asp">CREATE VIEW</a> query
    /// </summary>
    /// <param name="viewName">Name of the view the query will be generated for</param>
    /// <returns></returns>
    public static CreateViewQuery CreateView(string viewName) => new(viewName);

    /// <summary>
    /// Creates a <see cref="UpdateQuery"/> object suitable to build <a href="http://www.w3schools.com/sql/sql_update.asp">UPDATE</a> query
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns><see cref="UpdateQuery"/></returns>
    public static UpdateQuery Update(string tableName) => new(tableName);

    /// <summary>
    /// Creates a <see cref="SelectIntoQuery"/> object suitable to build <a href="http://www.w3schools.com/sql/sql_select_into.asp">SELECT INTO</a> query
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public static SelectIntoQuery SelectInto(string tableName) => new(tableName);

    /// <summary>
    /// Creates a <see cref="SelectQuery"/> object suitable to build <a href="http://www.w3schools.com/sql/sql_select.asp">SELECT</a> query
    /// </summary>
    /// <param name="columnNames">The column names.</param>
    /// <returns><see cref="SelectQuery"/></returns>
    /// <exception cref="System.ArgumentOutOfRangeException">if <paramref name="columnNames"/> is empty or contains only <see langword="null" /> elements.</exception>
    public static SelectQuery Select(params string[] columnNames) => new(columnNames);

    /// <summary>
    /// Creates a <see cref="SelectQuery"/> object suitable to build <a href="http://www.w3schools.com/sql/sql_select.asp">SELECT</a> query
    /// </summary>
    /// <param name="columns">The columns.</param>
    /// <returns><see cref="SelectQuery"/></returns>
    /// <exception cref="System.ArgumentOutOfRangeException">if <paramref name="columns"/> is empty or contains only <see langword="null" /> elements.</exception>
    public static SelectQuery Select(params IColumn[] columns) => new(columns);

    /// <summary>
    /// Creates a <see cref="TruncateQuery"/> object suitable to build <a href="http://www.w3schools.com/sql/sql_truncate.asp">SELECT</a> query
    /// </summary>
    /// <param name="tableName">Name of the table the <see cref="TruncateQuery"/> is built for</param>
    /// <returns><see cref="TruncateQuery"/></returns>
    /// <exception cref="System.ArgumentNullException">if <paramref name="tableName"/> is <see langword="null" />.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">if <paramref name="tableName"/> is empty or whitespace.</exception>
    public static TruncateQuery Truncate(string tableName) => new(tableName);

    /// <summary>
    /// Creates a <see cref="InsertIntoQuery"/> object suitable to build <a href="http://www.w3schools.com/sql/sql_insert.asp">INSERT INTO</a> or <a href="http://www.w3schools.com/sql/sql_insert_into_select.asp">INSERT INTO SELECT</a> query
    /// </summary>
    /// <param name="tableName">Name of the table the <see cref="InsertIntoQuery"/> is built for</param>
    /// <returns><see cref="InsertIntoQuery"/></returns>
    public static InsertIntoQuery InsertInto(string tableName) => new(tableName);

    /// <summary>
    /// Creates a <see cref="DeclareVariableQuery"/> object suitable to create a variable in a script
    /// </summary>
    /// <param name="variableName"></param>
    /// <returns></returns>
    public static DeclareVariableQuery Declare(string variableName) => new(variableName);

    /// <summary>
    /// Creates a <see cref="CasesColumn"/>.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="others"></param>
    /// <returns><see cref="CasesColumn"/></returns>
    /// <exception cref="ArgumentNullException"> if <paramref name="others"/> is <see langword="null" /></exception>
    public static CasesColumn Cases(WhenExpression first, params WhenExpression[] others)
    {
        if (first is null)
        {
            throw new ArgumentNullException(nameof(first));
        }

        if (others is null)
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
    /// <exception cref="ArgumentNullException">either <paramref name="criterion"/> or <paramref name="then"/> is <see langword="null" /></exception>
    public static WhenExpression When(WhereClause criterion, ColumnBase then) => new(criterion, then);
}