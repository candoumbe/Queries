using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Clauses;

/// <summary>
/// Contract to define a criterion
/// </summary>
/// <typeparam name="TColumn">Type of column the current instance can be applied onto</typeparam>
public interface IClause<TColumn> where TColumn : IColumn
{
    /// <summary>
    /// The column of the criterion
    /// </summary>
    TColumn Column { get;  }

    /// <summary>
    /// The operator of the criterion
    /// </summary>
    ClauseOperator Operator{ get; }

    /// <summary>
    /// The constraint enforced by the current criterion
    /// </summary>
    IColumn Constraint { get; }
}