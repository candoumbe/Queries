using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;

namespace Queries.Core.Builders;

/// <summary>
/// Maps a <see cref="FieldColumn"/> with a new value from a <see cref="Source"/>.
/// Used in <see cref="UpdateQuery"/>
/// </summary>
public class UpdateFieldValue : IEquatable<UpdateFieldValue>
{
    /// <summary>
    /// Defines the colum wich
    /// </summary>
    public ColumnBase Source { get; set; }

    /// <summary>
    /// Defines the <see cref="FieldColumn"/> onto which <see cref="Source"/> 
    /// </summary>
    public FieldColumn Destination { get; set; }

    /// <summary>
    /// Builds a new <see cref="UpdateFieldValue"/> instance.
    /// </summary>
    /// <param name="destination">Column to update</param>
    /// <param name="source">value to set</param>
    public UpdateFieldValue(FieldColumn destination, ColumnBase source)
    {
        Destination = destination ?? throw new ArgumentNullException(nameof(destination));
        Source = source;
    }

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as UpdateFieldValue);

    ///<inheritdoc/>
    public bool Equals(UpdateFieldValue other) => other != null && EqualityComparer<ColumnBase>.Default.Equals(Source, other.Source) && EqualityComparer<FieldColumn>.Default.Equals(Destination, other.Destination);

    ///<inheritdoc/>
    public override int GetHashCode()
    {
        int hashCode = 1918477335;
        hashCode = (hashCode * -1521134295) + EqualityComparer<ColumnBase>.Default.GetHashCode(Source);
        hashCode = (hashCode * -1521134295) + EqualityComparer<FieldColumn>.Default.GetHashCode(Destination);
        return hashCode;
    }
}