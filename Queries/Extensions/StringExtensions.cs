using Queries.Parts;
using Queries.Parts.Columns;

namespace Queries.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Turns the specified <paramref name="columnName"/> into <see cref="Parts.Columns.FieldColumn"/>
        /// </summary>
        /// <param name="columnName">Name of the field</param>
        /// <param name="alias">Alias of the field</param>
        /// <returns></returns>
        public static FieldColumn Field(this string columnName, string alias = null)
        {
            return new FieldColumn(columnName).As(alias);
        }

        /// <summary>
        /// Turns the specified <paramref name="tableName"/> into a <see cref="Parts.Table"/>
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="alias">Alias of the table</param>
        /// <returns><see cref="Table"/></returns>
        public static Table Table(this string tableName, string alias = null)
        {
            return new Table(tableName, alias);
        }
    }
}