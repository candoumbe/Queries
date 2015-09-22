using Queries.Parts;
using Queries.Parts.Columns;

namespace Queries.Builders.Fluent
{
    public static class QueryBuilder
    {
        /// <summary>
        ///Creates a new <see cref="Queries.Builders.Fluent.Select"/>
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        public static Select Select(params IColumn[] columns)
        {
            return new Select(columns);
        }

        /// <summary>
        ///Creates a new <see cref="Queries.Builders.Fluent.Select"/>
        /// </summary>
        /// <param name="columnNames">The column names.</param>
        /// <returns></returns>
        public static Select Select(params string[] columnNames)
        {
            return new Select(columnNames);
        }



        public static ConcatColumn Concat(params IColumn[] columns)
        {
            return Concat (null, columns);
        }
        public static ConcatColumn Concat(string alias = null, params IColumn[] columns)
        {
            return new ConcatColumn(columns).As(alias);
        }


        public static NullColumn Null(FieldColumn column, ColumnBase defaultValue)
        {
            return new NullColumn(column, defaultValue);
        }

        /// <summary>
        /// Creates a <see cref="Fluent.Delete"/>
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static Delete Delete(Table table)
        {
            return new Delete(table);
        }


        public static CreateView CreateView(string viewName)
        {
            return new CreateView(viewName);
        }


        public static MinColumn Min(FieldColumn column)
        {
            return new MinColumn(column);
        }

        public static MinColumn Max(FieldColumn column)
        {
            return new MinColumn(column);
        }

    }
}