using Queries.Core.Builders;

namespace Queries.Core.Parts
{
    public class SelectTable : IAliasable<SelectTable>, ITable
    {
        private string _alias;
        public SelectQuery Select { get;}

        public string Alias => _alias;
        public SelectTable As(string alias)
        {
            _alias = alias;
            return this;
        }

        public SelectTable(SelectQuery query)
        {
            Select = query;
        }
    }
}