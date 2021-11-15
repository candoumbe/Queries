#if NET6_0_OR_GREATER
namespace Queries.Core.Parts.Columns;

using System;

/// <summary>
/// A column that holds a <see cref="DateOnly"/>
/// </summary>
public class DateColumn : Literal, IFormattableColumn<DateColumn>
{
    /// <summary>
    /// The format currently applied for this column.
    /// </summary>
    public string StringFormat { get; private set; }

    /// <summary>
    /// Builds a new <see cref="DateColumn"/> instance.
    /// </summary>
    /// <param name="date">value hold by the column.</param>
    /// <param name="format">Format of the column</param>
    public DateColumn(DateOnly date, string format = "yyyy-MM-dd") : base(date)
    {
        StringFormat = format;
    }

    ///<inheritdoc/>
    public DateColumn Format(string format)
    {
        StringFormat = format;
        return this;
    }
}

#endif