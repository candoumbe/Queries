using Queries.Core.Parts.Clauses;

namespace Queries.Core.Parts.Joins
{
    /// <summary>
    /// Base class for Join operator
    /// </summary>
    public abstract class JoinBase : IJoin
    {
        public JoinType JoinType { get; }
        public Table Table { get;  }
        public IWhereClause On { get; }

        /// <summary>
        /// Builds a new <see cref="JoinBase"/> instance.
        /// </summary>
        /// <param name="joinType">The JOIN operator type</param>
        /// <param name="table">The table onto which the operator will be apply</param>
        /// <param name="on">The "ON" clause</param>
        protected JoinBase(JoinType joinType, Table table, IWhereClause on)
        {
            JoinType = joinType;
            Table = table;
            On = @on;
        }
    }
}