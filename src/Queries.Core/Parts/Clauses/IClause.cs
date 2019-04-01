using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Clauses
{
    public interface IClause<T> where T : IColumn
    {
        T Column { get;  }
        ClauseOperator Operator{ get; }
        IColumn Constraint { get; }
    }
}