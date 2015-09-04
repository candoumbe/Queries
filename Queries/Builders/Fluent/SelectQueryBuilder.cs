using Queries.Parts.Columns;

namespace Queries.Builders.Fluent
{
    public static class QueryBuilders
    {
        public static IColumn Min(string columnName)
        {
            return new Min(columnName);
        }

        public static IColumn Max(string columnName)
        {
            return new Max(TableColumn.From(columnName));
        }

        

    }
}