using System.Collections.Generic;

namespace Queries.Parts.Clauses
{
    public class CompositeWhereClause : IWhereClause
    {
        public ClauseLogic Logic { get; set; }
        public IList<IWhereClause> Clauses { get; set; }

        public CompositeWhereClause()
        {
            Clauses = new List<IWhereClause>();
        }
    }
}