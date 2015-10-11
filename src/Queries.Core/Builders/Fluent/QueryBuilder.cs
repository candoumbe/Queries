using Queries.Core.Parts;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Builders.Fluent
{
    public static class QueryBuilder
    {
        
#region Columns conversions shortcuts

        
        public static LengthColumn Length(IColumn column) => new LengthColumn(column);


        public static ConcatColumn Concat(params IColumn[] columns) => new ConcatColumn(columns);


        public static NullColumn Null(FieldColumn column, ColumnBase defaultValue) => new NullColumn(column, defaultValue);


        public static CountColumn Count(FieldColumn column) => new CountColumn(column);

        public static NullColumn Null(LiteralColumn column, ColumnBase defaultValue) => new NullColumn(column, defaultValue);


        
        public static MinColumn Min(IColumn column) => new MinColumn(column);

        public static MinColumn Min(string columnName) => new MinColumn(columnName);

        public static AvgColumn Avg(IColumn column) => new AvgColumn(column);

        public static AvgColumn Avg(string columnName) => new AvgColumn(columnName);

        /// <summary>
        /// Creates a new <see cref="SubstringColumn"/>
        /// </summary>
        /// <param name="column">Column to which the function will be applied</param>
        /// <param name="start">position where the substring will start</param>
        /// <param name="length">position where the substring will end</param>
        /// <returns><see cref="SubstringColumn"/></returns>
        public static SubstringColumn Substring(IColumn column, int start, int? length = null) => new SubstringColumn(column, start, length);


        /// <summary>
        /// Wraps a <see cref="IColumn"/> into <see cref="MaxColumn"/> function
        /// </summary>
        /// <param name="column"></param>
        /// <returns><see cref="MaxColumn"/></returns>
        public static MaxColumn Max(IColumn column) => new MaxColumn(column);

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
        /// <returns></returns>
        public static SelectQuery Select(params string[] columnNames) => new SelectQuery(columnNames);

        /// <summary>
        /// Creates a <see cref="SelectQuery"/> object suitable to build <a href="http://www.w3schools.com/sql/sql_select.asp">SELECT</a> query
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns><see cref="SelectQuery"/></returns>
        public static SelectQuery Select(params IColumn[] columns) => new SelectQuery(columns);

        /// <summary>
        /// Creates a <see cref="TruncateQuery"/> object suitable to build <a href="http://www.w3schools.com/sql/sql_truncate.asp">SELECT</a> query
        /// </summary>
        /// <param name="tableName">Name of the table the <see cref="TruncateQuery"/> is built for</param>
        /// <returns><see cref="TruncateQuery"/></returns>
        public static TruncateQuery Truncate(string tableName) => new TruncateQuery(tableName);

        /// <summary>
        /// Creates a <see cref="TruncateQuery"/> object suitable to build <a href="http://www.w3schools.com/sql/sql_truncate.asp">SELECT</a> query
        /// </summary>
        /// <param name="tableName">Name of the table the <see cref="TruncateQuery"/> is built for</param>
        /// <returns><see cref="TruncateQuery"/></returns>
        public static InsertIntoQuery InsertInto(string tableName) => new InsertIntoQuery(tableName);

    }
}