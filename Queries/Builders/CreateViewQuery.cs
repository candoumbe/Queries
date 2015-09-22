namespace Queries.Builders
{
    public class CreateViewQuery
    {
        public string Name { get; set; }
        
        public SelectQuery Select { get; set; }
    }
}
