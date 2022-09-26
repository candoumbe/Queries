namespace Queries.Core.Parts.Columns;

/// <summary>
/// Represents a UUID value
/// </summary>
public sealed class UniqueIdentifierValue : ColumnBase
{
    ///<inheritdoc/>
    public override IColumn Clone() => new UniqueIdentifierValue();

    ///<inheritdoc/>
    public override bool Equals(ColumnBase other) => other is UniqueIdentifierValue;
}
