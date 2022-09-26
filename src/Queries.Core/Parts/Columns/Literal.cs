using Queries.Core.Builders;
using System;

namespace Queries.Core.Parts.Columns;

/// <summary>
/// A column that render is content "as is" with no interpretation.
/// </summary>
public class Literal : ColumnBase, IAliasable<Literal>, IInsertable, IEquatable<Literal>
{
    /// <summary>
    /// Raw value the current instance holds.
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Builds a <see cref="Literal"/> instance.
    /// </summary>
    /// <param name="value"></param>
    public Literal(object value = null)
    {
        Value = value switch
        {
            int i => i,
            float f => f,
            decimal d => d,
            double d => d,
            long l => l,
            bool b => b,
            string s => s,
            DateTime dateTime => dateTime,
            DateTimeOffset dateTimeOffset => dateTimeOffset,
#if NET6_0_OR_GREATER
            DateOnly date => date,
            TimeOnly time => time,
#endif
            null => null,
            _ => throw new NotSupportedException(
#if NET6_0_OR_GREATER
                                                     $"only bool/int/float/decimal/double/long/string/Datetime/DateTimeOffset/DateOnly/TimeOnly are supported"), 
#else
                                                     "only bool/int/float/decimal/double/long/string/Datetime/DateTimeOffset are supported"), 
#endif
        };
    }

    private string _alias;

    ///<inheritdoc/>
    public string Alias => _alias;

    ///<inheritdoc/>
    public Literal As(string alias)
    {
        _alias = alias;

        return this;
    }

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as Literal);

    ///<inheritdoc/>
    public bool Equals(Literal other) => other != null
        && (other.Value?.Equals(Value) ?? false)
        && (Alias == other.Alias);

    ///<inheritdoc/>
    public override bool Equals(ColumnBase other) => Equals(other as Literal);

    ///<inheritdoc/>
    public override int GetHashCode() => (Value, Alias).GetHashCode();

    ///<inheritdoc/>
    public override IColumn Clone() => new Literal(Value);

    ///<inheritdoc/>
    public override string ToString() => this.Jsonify();

    ///<inheritdoc/>
    public static implicit operator Literal(bool value) => new BooleanColumn(value);

    ///<inheritdoc/>
    public static implicit operator Literal(string value) => new StringColumn(value);

    ///<inheritdoc/>
    public static implicit operator Literal(int value) => new NumericColumn(value);

    ///<inheritdoc/>
    public static implicit operator Literal(float value) => new NumericColumn(value);

    ///<inheritdoc/>
    public static implicit operator Literal(short value) => new NumericColumn(value);

    ///<inheritdoc/>
    public static implicit operator Literal(long value) => new NumericColumn(value);
}
