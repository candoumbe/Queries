using System.Collections.Generic;
using Queries.Parts;
using Queries.Parts.Columns;
using Queries.Parts.Joins;

namespace Queries.Builders
{
    public abstract class SelectQueryBase
    {
        public IList<IColumn> Select { get; set; }
        public IClause Where { get; set; }
        public IList<IJoin> Joins { get; set; }
        public IList<SelectQuery> Union { get; set; }
        
        protected SelectQueryBase()
        {
            Select = new List<IColumn>();
            Joins = new List<IJoin>();
            Union = new List<SelectQuery>();
        }
    }
}