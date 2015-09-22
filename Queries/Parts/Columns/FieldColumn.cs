namespace Queries.Parts.Columns
{
    public class FieldColumn : ColumnBase, INamable, IAliasable
    {
        public string Name { get; set; }

        public string Alias { get; set; }

    }
}