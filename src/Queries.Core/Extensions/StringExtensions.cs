using Queries.Core.Builders;
using Queries.Core.Parts;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Sorting;

namespace System
{
    public static class StringExtensions
    {
        /// <summary>
        /// Turns the specified <paramref name="columnName"/> into <see cref="FieldColumn"/>.
        /// </summary>
        /// <param name="columnName">Name of the field</param>
        /// <param name="alias">optional alias of the <paramref name="columnName"/></param>
        /// <returns></returns>
        public static FieldColumn Field(this string columnName, string alias) => new FieldColumn(columnName).As(alias);

        /// <summary>
        /// Turns the specified <paramref name="columnName"/> into <see cref="FieldColumn"/>.
        /// </summary>
        /// <param name="columnName">Name of the field</param>
        /// <returns></returns>
        public static FieldColumn Field(this string columnName) => columnName.Field(null);

        /// <summary>
        /// Turns the specified <paramref name="tableName"/> into a <see cref="Queries.Core.Parts.Table"/>
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="alias">Alias of the table</param>
        /// <returns><see cref="Table"/></returns>
        public static Table Table(this string tableName, string alias) => new Table(tableName, alias);

        /// <summary>
        /// Turns the specified <paramref name="tableName"/> into a <see cref="Queries.Core.Parts.Table"/>
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <returns><see cref="Table"/></returns>
        public static Table Table(this string tableName) => tableName.Table(null);


        /// <summary>
        /// Create an <see cref="InsertedValue"/> instance.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="column"></param>
        /// <returns><see cref="InsertedValue"/></returns>
        public static InsertedValue InsertValue(this string columnName, IColumn column)
            => columnName.Field().InsertValue(column);

        public static IOrder Asc(this string columnNameOrFieldAlias) => new OrderExpression(columnNameOrFieldAlias.Field());

        public static IOrder Desc(this string columnNameOrFieldAlias) => new OrderExpression(columnNameOrFieldAlias.Field(), OrderDirection.Descending);

        /// <summary>
        /// Turns <paramref name="nativeQuery"/> into <see cref="NativeQuery"/>.
        /// </summary>
        /// <param name="nativeQuery">The raw string</param>
        /// <returns></returns>
        public static NativeQuery AsNative(this string nativeQuery) => new NativeQuery(nativeQuery);
    }
}