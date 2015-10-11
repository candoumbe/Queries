using System.Collections.Generic;

namespace Queries.Core.Parts.Clauses
{
    public class CompositeHavingClause : IHavingClause
    {
        public ClauseLogic Logic { get; set; }
        public IList<IHavingClause> Clauses { get; set; }

        public CompositeHavingClause()
        {
            Clauses = new List<IHavingClause>();
        }
    }
}