using System.Collections.Generic;
using Queries.Core.Builders.Fluent;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Joins;
using Queries.Core.Parts.Sorting;

namespace Queries.Core.Builders
{
    public abstract class SelectQueryBase : IDataManipulationQuery, IInsertable
    {
        internal IList<IColumn> Columns { get; set; }
        public IWhereClause WhereCriteria { get; set; }
        public IHavingClause HavingCriteria { get; set; }
        public IList<IJoin> Joins { get; set; }
        public IList<ISort> Sorts { get; set; } 
        
        protected SelectQueryBase()
        {
            Columns = new List<IColumn>();
            Joins = new List<IJoin>();
            Sorts = new List<ISort>();
        }
    }
}