using System;

namespace Queries.Core.Parts.Columns;

/// <summary>
/// A column that can hold a <see langword="string"/> value.
/// </summary>
public class StringColumn : Literal, IEquatable<StringColumn>
{
    /// <summary>
    /// Builds a new <see cref="StringColumn"/> instance.
    /// </summary>
    /// <param name="value"></param>
    public StringColumn(string value = "")
        : base(value)
    {}

    ///<inheritdoc/>
    public override IColumn Clone() => new StringColumn((string)Value).As(Alias);

    ///<inheritdoc/>
    public bool Equals(StringColumn other) => (Value, Alias).Equals((other?.Value, other?.Alias));

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as StringColumn);

    ///<inheritdoc/>
#if !NETSTANDARD2_1
    public override int GetHashCode() => (Value, Alias).GetHashCode();
#else
    public override int GetHashCode() => HashCode.Combine(Value, Alias);
#endif

    ///<inheritdoc/>
    public override string ToString() => this.Jsonify();

    ///<inheritdoc/>
    public static implicit operator StringColumn(string input) => new(input);
}