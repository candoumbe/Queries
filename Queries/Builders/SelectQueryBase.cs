using System.Collections.Generic;
using Queries.Parts.Clauses;
using Queries.Parts.Columns;
using Queries.Parts.Joins;
using Queries.Parts.Sorting;

namespace Queries.Builders
{
    public abstract class SelectQueryBase
    {
        public IList<IColumn> Select { get; set; }
        public IWhereClause Where { get; set; }
        public IHavingClause Having { get; set; }
        public IList<IJoin> Joins { get; set; }
        public IList<SelectQuery> Union { get; set; }
        public IList<ISort> OrderBy { get; set; } 
        
        protected SelectQueryBase()
        {
            Select = new List<IColumn>();
            Joins = new List<IJoin>();
            Union = new List<SelectQuery>();
            OrderBy = new List<ISort>();
        }
    }
}