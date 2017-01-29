using Queries.Core.Parts.Clauses;

namespace Queries.Core.Parts.Joins
{
    public abstract class JoinBase : IJoin
    {
        public JoinType JoinType { get; set; }
        public Table Table { get; set; }
        public IWhereClause On { get; set; }

        protected JoinBase(JoinType joinType, Table table, IWhereClause on)
        {
            JoinType = joinType;
            Table = table;
            On = @on;
        }
    }
}