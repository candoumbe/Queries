using Queries.Builders;

namespace Queries.Parts
{
    public class SelectTable : ITable
    {
        public SelectQuery Select { get; set; }
        public string Alias { get; set; }
    }
}