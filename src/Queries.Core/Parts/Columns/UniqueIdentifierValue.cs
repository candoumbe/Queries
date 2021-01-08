namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// Represents a UUID value
    /// </summary>
    public sealed class UniqueIdentifierValue : ColumnBase
    {
        public override IColumn Clone() => new UniqueIdentifierValue();

        public override bool Equals(ColumnBase other) => other is UniqueIdentifierValue;
    }
}
