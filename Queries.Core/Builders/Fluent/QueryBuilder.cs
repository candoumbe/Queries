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
        /// <returns></returns>
        public static SelectQuery Select(params IColumn[] columns)
        {
            return new SelectQuery(columns);
        }


       

        /// <summary>
        ///Creates a new <see cref="SelectQuery"/>
        /// </summary>
        /// <param name="columnNames">The column names.</param>
        /// <returns></returns>
        public static SelectQuery Select(params string[] columnNames)
        {
            return new SelectQuery(columnNames);
        }


        public static LengthColumn Length(StringColumn column)
        {
            return new LengthColumn(column);
        }


        public static LengthColumn Length(FieldColumn column)
        {
            return new LengthColumn(column);
        }


        
        public static ConcatColumn Concat(params IColumn[] columns)
        {
            return new ConcatColumn(columns);
        }


        public static NullColumn Null(FieldColumn column, ColumnBase defaultValue)
        {
            return new NullColumn(column, defaultValue);
        }

        public static CountColumn Count(FieldColumn column)
        {
            return new CountColumn(column);
        }

        public static NullColumn Null(LiteralColumn column, ColumnBase defaultValue)
        {
            return new NullColumn(column, defaultValue);
        }


        

        /// <summary>
        /// Creates a <see cref="DeleteQuery"/>
        /// </summary>
        /// <param name="tableName">Name of the table to be deleted</param>
        /// <returns></returns>
        public static DeleteQuery Delete(string tableName)
        {
            return new DeleteQuery(tableName);
        }


        public static CreateViewQuery CreateView(string viewName)
        {
            return new CreateViewQuery(viewName);
        }


        public static MinColumn Min(FieldColumn column)
        {
            return new MinColumn(column);
        }

        public static MinColumn Min(string columnName)
        {
            return new MinColumn(columnName);
        }

        public static MaxColumn Max(FieldColumn column)
        {
            return new MaxColumn(column);
        }

    }
}