using Queries.Parts;
using Queries.Parts.Columns;

namespace Queries.Extensions
{
    public static class StringExtensions
    {
        public static FieldColumn Field(this string columnName, string alias = null)
        {
            return new FieldColumn() {Name = columnName, Alias = alias};
        }

        public static Table Table(this string tableName, string alias = null)
        {
            return new Table() { Name = tableName, Alias = alias};
        }
    }
}