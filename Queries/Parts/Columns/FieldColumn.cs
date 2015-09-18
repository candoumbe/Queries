namespace Queries.Parts.Columns
{
    public class FieldColumn : ColumnBase, INamable, IAliasable
    {
        public string Name { get; set; }

        public string Alias { get; set; }


        public static FieldColumn From(string name, string alias = "")
        {
            return new Field(){Name = name, Alias = alias};
        }
    }
}