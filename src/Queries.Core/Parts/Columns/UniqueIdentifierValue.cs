namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// Represents a UUID value
    /// </summary>
    public sealed class UniqueIdentifierValue : ColumnBase, IColumn
    {
        public override IColumn Clone() => new UniqueIdentifierValue();
    }
}
