namespace Queries.Builders
{
    public class CreateViewQuery 
    {
        public string Name { get; set; }
        
        public SelectQuery As { get; set; }

        internal CreateViewQuery()
        {}

        public static implicit operator CreateViewQuery(CreateView query) => query?.Build();
    }
}
