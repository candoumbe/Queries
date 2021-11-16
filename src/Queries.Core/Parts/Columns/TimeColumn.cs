#if NET6_0_OR_GREATER
namespace Queries.Core.Parts.Columns;

using System;

/// <summary>
/// A column that holds a <see cref="TimeOnly"/>
/// </summary>
public class TimeColumn : Literal, IFormattableColumn<TimeColumn>
{
    /// <summary>
    /// The format currently applied for this column.
    /// </summary>
    public string StringFormat { get; private set; }

    /// <summary>
    /// Builds a new <see cref="TimeColumn"/> instance.
    /// </summary>
    /// <param name="time">value hold by the column.</param>
    /// <param name="format">Format of the column</param>
    public TimeColumn(TimeOnly time, string format = "HH:mm:ss.FFFFFFF") : base(time)
    {
        StringFormat = format;
    }

    ///<inheritdoc/>
    public TimeColumn Format(string format)
    {
        StringFormat = format;
        return this;
    }
}

#endif