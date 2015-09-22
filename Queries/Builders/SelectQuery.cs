using System.Collections.Generic;
using Queries.Parts;

namespace Queries.Builders
{
    public class SelectQuery : SelectQueryBase
    {

        public int? Limit { get; set; }
        public IList<ITable> From { get; set; }

        internal SelectQuery()
        {
            From = new List<ITable>();
        }
    }

}