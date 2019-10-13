using Queries.Core.Parts;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queries.Renderers.Postgres.Parts.Columns
{
    /// <summary>
    /// A column that contains a valid JSON string
    /// </summary>
    public class JsonFieldColumn : ColumnBase, IAliasable<JsonFieldColumn>
    {
        /// <summary>
        /// Builds a new <see cref="JsonFieldColumn"/> instance
        /// </summary>
        /// <param name="column">Column that holds the value</param>
        /// <param name="path">Path to the JSON property</param>
        /// <param name="renderAsString"><c>true</c>  if the field should be rendered as string</param>
        /// <exception cref="ArgumentNullException">either <paramref name="column"/> or <paramref name="path"/> is <c>null</c>.</exception>
        public JsonFieldColumn(FieldColumn column, string path, bool renderAsString = false)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            RenderAsString = renderAsString;
        }

        public FieldColumn Column { get; }
        public string Path { get; }
        public bool RenderAsString { get; }

        public string Alias { get; private set; }

        public override IColumn Clone() => new JsonFieldColumn(Column.Clone() as FieldColumn, Path).As(Alias);

        public override bool Equals(object obj) => Equals(obj as JsonFieldColumn);

        public override bool Equals(ColumnBase other) => other is JsonFieldColumn json && (Alias,Path, Column).Equals((json.Alias,json.Path, json.Column));

        public override int GetHashCode() => (Alias, Path, Column).GetHashCode();

        public static bool operator ==(JsonFieldColumn left, JsonFieldColumn right) => left?.Equals(right) ?? false;

        public static bool operator !=(JsonFieldColumn left, JsonFieldColumn right)
        {
            return !(left == right);
        }

        public JsonFieldColumn As(string alias)
        {
            Alias = alias;

            return this;
        }
    }
}
