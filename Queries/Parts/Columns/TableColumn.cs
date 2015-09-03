namespace Queries.Parts.Columns
{
    public class TableColumn : ColumnBase, INamable, IAliasable
    {
        public string Name { get; set; }

        public string Alias { get; set; }


        public static TableColumn From(string name, string alias = "")
        {
            return new TableColumn(){Name = name, Alias = alias};
        }
    }
}
