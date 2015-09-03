using System.Collections.Generic;
using Queries.Parts;

namespace Queries.Builders
{
    public class SelectQuery : SelectQueryBase
    {

        public int? Limit { get; set; }
        public IList<TableTerm> From { get; set; }

        public SelectQuery()
        {
            From = new List<TableTerm>();
        }
    }

}