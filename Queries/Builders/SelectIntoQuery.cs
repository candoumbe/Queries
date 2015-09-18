using System.Collections.Generic;
using Queries.Parts;

namespace Queries.Builders
{
    public class SelectIntoQuery : SelectQueryBase
    {
        public Table Into { get; set; }

        public ITable From { get; set; }


        public SelectIntoQuery()
        {
            Union = new List<SelectQuery>();
        }
    }

}