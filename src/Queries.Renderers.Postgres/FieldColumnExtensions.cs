using Queries.Renderers.Postgres.Parts.Columns;

namespace Queries.Core.Parts.Columns
{
    public static class FieldColumnExtensions
    {
        public static JsonFieldColumn Json(this FieldColumn column, string path = null) => new JsonFieldColumn(column, path);
    }
}
