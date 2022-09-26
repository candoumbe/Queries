using Queries.Core.Parts;
using Queries.Core.Parts.Columns;

using System;

namespace Queries.Renderers.Postgres.Parts.Columns;

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
    /// <exception cref="ArgumentNullException">either <paramref name="column"/> or <paramref name="path"/> is <see langword="null" />.</exception>
    public JsonFieldColumn(FieldColumn column, string path, bool renderAsString = false)
    {
        Column = column ?? throw new ArgumentNullException(nameof(column));
        Path = path ?? throw new ArgumentNullException(nameof(path));
        RenderAsString = renderAsString;
    }

    /// <summary>
    /// The original column that was turned into a <see cref="JsonFieldColumn"/>.
    /// </summary>
    public FieldColumn Column { get; }

    /// <summary>
    /// Json path of the selection
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// A hint to render the column's content as a string
    /// </summary>
    public bool RenderAsString { get; }

    ///<inheritdoc/>
    public string Alias { get; private set; }

    ///<inheritdoc/>
#if NET6_0_OR_GREATER
    public override IColumn Clone() => new JsonFieldColumn(Column.Clone(), Path).As(Alias);
#else
    public override IColumn Clone() => new JsonFieldColumn((FieldColumn) Column.Clone(), Path).As(Alias);
#endif

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as JsonFieldColumn);

    ///<inheritdoc/>
    public override bool Equals(ColumnBase other) => other is JsonFieldColumn json && (Alias,Path, Column).Equals((json.Alias,json.Path, json.Column));

    ///<inheritdoc/>
    public override int GetHashCode() => (Alias, Path, Column).GetHashCode();

    public static bool operator ==(JsonFieldColumn left, JsonFieldColumn right) => left?.Equals(right) ?? false;

    ///<inheritdoc/>
    public static bool operator !=(JsonFieldColumn left, JsonFieldColumn right) => !(left == right);

    ///<inheritdoc/>
    public JsonFieldColumn As(string alias)
    {
        Alias = alias;

        return this;
    }
}
