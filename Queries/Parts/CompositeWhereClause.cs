using System.Collections.Generic;

namespace Queries.Parts
{
    public class CompositeWhereClause : IClause
    {
        public WhereLogic Logic { get; set; }
        public IList<IClause> Clauses { get; set; }

        public CompositeWhereClause()
        {
            Clauses = new List<IClause>();
        }
    }
}