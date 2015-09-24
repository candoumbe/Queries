using System.Collections.Generic;
using Queries.Core.Parts;

namespace Queries.Core.Builders
{
    public class SelectIntoQuery : SelectQueryBase
    {
        public Table Into { get; set; }

        public ITable From { get; set; }


        public SelectIntoQuery()
        {
            
        }
    }

}