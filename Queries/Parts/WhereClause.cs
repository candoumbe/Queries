using System;
using System.Collections.Generic;
using Queries.Parts.Columns;

namespace Queries.Parts
{
    public class WhereClause : IClause
    {
        public IColumn Column { get; set; }
        public WhereOperator Operator{ get; set; }
        public ColumnBase Constraint { get; set; }
        public IList<Tuple<WhereLogic, IList<WhereClause>>> Clauses { get; set; }

        public WhereClause()
        {
            Clauses = new List<Tuple<WhereLogic, IList<WhereClause>>>();
        }
    }
}
