using System.Collections.Generic;
using System.Linq;

namespace Queries.Core.Parts.Clauses
{
    public class CompositeHavingClause : IHavingClause
    {
        public ClauseLogic Logic { get; set; }
        public IEnumerable<IHavingClause> Clauses { get; set; }

        public CompositeHavingClause() => Clauses = Enumerable.Empty<IHavingClause>();
    }
}