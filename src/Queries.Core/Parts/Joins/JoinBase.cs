using Queries.Core.Parts.Clauses;

namespace Queries.Core.Parts.Joins;

/// <summary>
/// Base class for Join operator
/// </summary>
public abstract class JoinBase : IJoin
{
    /// <summary>
    /// The type of the performed join
    /// </summary>
    public JoinType JoinType { get; }

    /// <summary>
    /// Table onto which the join operation will be performed
    /// </summary>
    public Table Table { get;  }

    /// <summary>
    /// The criterion of the join.
    /// </summary>
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