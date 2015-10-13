﻿using Queries.Core.Builders;
using Queries.Core.Parts;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Turns the specified <paramref name="columnName"/> into <see cref="Parts.Columns.FieldColumn"/>
        /// </summary>
        /// <param name="columnName">Name of the field</param>
        /// <returns></returns>
        public static FieldColumn Field(this string columnName)
        {
            return new FieldColumn(columnName);
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


        public static InsertedValue InsertValue(this string columnName, LiteralColumn value)
            => new InsertedValue(columnName.Field(), value);


        public static InsertedValue InsertValue(this FieldColumn columnName, LiteralColumn value)
            => new InsertedValue(columnName, value);

        

    }
}