using System.Collections.Generic;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Joins;
using Queries.Core.Parts.Sorting;
using Queries.Core.Attributes;

namespace Queries.Core.Builders
{
    /// <summary>
    /// Base class for queries that select data
    /// </summary>
    [DataManipulationLanguage]
    public abstract class SelectQueryBase : IInsertable, IQuery
    {
        public IList<IColumn> Columns { get; protected set; }
        public IWhereClause WhereCriteria { get; protected set; }
        public IHavingClause HavingCriteria { get; protected set; }
        public IList<IJoin> Joins { get; protected set; }
        public IList<ISort> Sorts { get; protected set; } 
        
        /// <summary>
        /// Builds a new <see cref="SelectQueryBase"/> instance.
        /// </summary>
        protected SelectQueryBase()
        {
            Columns = new List<IColumn>();
            Joins = new List<IJoin>();
            Sorts = new List<ISort>();
        }
    }
}