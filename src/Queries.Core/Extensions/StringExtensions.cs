using Queries.Core.Parts;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Sorting;

namespace System
{
    public static class StringExtensions
    {
        /// <summary>
        /// Turns the specified <paramref name="columnName"/> into <see cref="Parts.Columns.FieldColumn"/>
        /// </summary>
        /// <param name="columnName">Name of the field</param>
        /// <returns></returns>
        public static FieldColumn Field(this string columnName) => new FieldColumn(columnName);

        /// <summary>
        /// Turns the specified <paramref name="tableName"/> into a <see cref="Queries.Core.Parts.Table"/>
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="alias">Alias of the table</param>
        /// <returns><see cref="Table"/></returns>
        public static Table Table(this string tableName, string alias = null) => new Table(tableName, alias);

        /// <summary>
        /// Create an <see cref="InsertedValue"/> instance.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <see cref="FieldColumnExtensions.InsertValue(IColumn)"/>
        public static InsertedValue InsertValue(this string columnName, IColumn column)
            => columnName.Field().InsertValue(column);

        public static ISort Asc(this string columnNameOrFieldAlias) => new SortExpression(columnNameOrFieldAlias.Field());

        public static ISort Desc(this string columnNameOrFieldAlias) => new SortExpression(columnNameOrFieldAlias.Field(), SortDirection.Descending);
    }
}