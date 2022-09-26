using Queries.Core.Builders;
using System;

namespace Queries.Core.Parts.Columns;

/// <summary>
/// A column that contains data
/// </summary>
public class FieldColumn : ColumnBase, INamable, IAliasable<FieldColumn>, IInsertable, IEquatable<FieldColumn>
{
    /// <summary>
    /// Name of the column
    /// </summary>
    public string Name { get; set; }

    private string _alias;

    ///<inheritdoc/>
    public string Alias => _alias;

    /// <summary>
    /// Builds a new <see cref="FieldColumn"/> which has the specified <paramref name="columnName"/>.
    /// </summary>
    /// <param name="columnName"></param>
    /// <exception cref="ArgumentNullException"><paramref name="columnName"/> is <see langword="null"/></exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="columnName"/> is empty or only contains whitespaces.</exception>
    public FieldColumn(string columnName)
    {
        Name = columnName switch
        {
            { Length: 0} or => throw new ArgumentOutOfRangeException(nameof(columnName), columnName, $"{nameof(columnName)} cannot be empty or whitespace only"),
            string value when string.IsNullOrWhiteSpace(value) => throw new ArgumentOutOfRangeException(nameof(columnName)),
            null => throw new ArgumentNullException(nameof(columnName), $"{nameof(columnName)} cannot be null"),
            _ => columnName
        };
    }

    ///<inheritdoc/>
    public FieldColumn As(string alias)
    {
        _alias = alias;

        return this;
    }

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as FieldColumn);

    ///<inheritdoc/>
    public bool Equals(FieldColumn other) => (Name, Alias).Equals((other?.Name, other?.Alias));

    ///<inheritdoc/>
    public override bool Equals(ColumnBase other) => Equals(other as FieldColumn);

    ///<inheritdoc/>
#if !NETSTANDARD2_1
    public override int GetHashCode() => (Name, Alias).GetHashCode();
#else
    public override int GetHashCode() => HashCode.Combine(Name, Alias);
#endif
    ///<inheritdoc/>
    public override string ToString() => this.Jsonify();

    /// <summary>
    /// Performs a deep copy of the current instance
    /// </summary>
    /// <returns></returns>
#if NET5_0_OR_GREATER
    public override FieldColumn Clone() => new FieldColumn(Name);
#else
    public override IColumn Clone() => new FieldColumn(Name);
#endif
}