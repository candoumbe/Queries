using Queries.Core.Parts;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Builders.Fluent
{
    public static class QueryBuilder
    {
        /// <summary>
        ///Creates a new <see cref="SelectQuery"/>
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns><see cref="SelectQuery"/></returns>
        public static SelectQuery Select(params IColumn[] columns) => new SelectQuery(columns);


        /// <summary>
        ///Creates a new <see cref="SelectQuery"/>
        /// </summary>
        /// <param name="columnNames">The column names.</param>
        /// <returns></returns>
        public static SelectQuery Select(params string[] columnNames) => new SelectQuery(columnNames);


        public static LengthColumn Length(IColumn column) => new LengthColumn(column);


        public static ConcatColumn Concat(params IColumn[] columns) => new ConcatColumn(columns);


        public static NullColumn Null(FieldColumn column, ColumnBase defaultValue) => new NullColumn(column, defaultValue);


        public static CountColumn Count(FieldColumn column) => new CountColumn(column);

        public static NullColumn Null(LiteralColumn column, ColumnBase defaultValue) => new NullColumn(column, defaultValue);


        /// <summary>
        /// Creates a <see cref="DeleteQuery"/>
        /// </summary>
        /// <param name="tableName">Name of the table to be deleted</param>
        /// <returns></returns>
        public static DeleteQuery Delete(string tableName) => new DeleteQuery(tableName);

        /// <summary>
        /// Creates a <see cref="CreateViewQuery"/>
        /// </summary>
        /// <param name="viewName">Name of the view the query will be generated for</param>
        /// <returns></returns>
        public static CreateViewQuery CreateView(string viewName) => new CreateViewQuery(viewName);


        public static MinColumn Min(IColumn column) => new MinColumn(column);

        public static MinColumn Min(string columnName) => new MinColumn(columnName);

        public static AvgColumn Avg(IColumn column) => new AvgColumn(column);

        public static AvgColumn Avg(string columnName) => new AvgColumn(columnName);

        /// <summary>
        /// Wraps a <see cref="IColumn"/> into <see cref="MaxColumn"/> function
        /// </summary>
        /// <param name="column"></param>
        /// <returns><see cref="MaxColumn"/></returns>
        public static MaxColumn Max(IColumn column) => new MaxColumn(column);


        /// <summary>
        /// Creates an "UPDATE" query object
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns><see cref="UpdateQuery"/></returns>
        public static UpdateQuery Update(string tableName) => new UpdateQuery(tableName);

        public static SelectIntoQuery SelectInto(string tableName) => new SelectIntoQuery(tableName);

       
    }
}