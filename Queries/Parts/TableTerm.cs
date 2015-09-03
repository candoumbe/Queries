namespace Queries.Parts
{
    public class TableTerm : IAliasable, INamable
    {
        public string Alias { get; set; }

        public string Name { get; set; }
    }
}
