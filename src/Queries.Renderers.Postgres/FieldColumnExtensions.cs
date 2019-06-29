using Queries.Renderers.Postgres.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queries.Core.Parts.Columns
{
    public static class FieldColumnExtensions
    {
        public static JsonFieldColumn Json(this FieldColumn column, string path = null) => new JsonFieldColumn(column, path);
    }
}
