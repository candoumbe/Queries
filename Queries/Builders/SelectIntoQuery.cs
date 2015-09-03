using System.Collections.Generic;
using System.Data.Common;
using Queries.Parts;
using Queries.Parts.Columns;
using Queries.Parts.Joins;

namespace Queries.Builders
{
    public class SelectIntoQuery : SelectQueryBase
    {
        public TableTerm Into { get; set; }

        public SelectQuery FromSelect { get; set; }

        public IList<TableTerm> FromTable { get; set; }

        public SelectIntoQuery()
        {
            Union = new List<SelectQuery>();
            FromTable = new List<TableTerm>();
        }
    }

}