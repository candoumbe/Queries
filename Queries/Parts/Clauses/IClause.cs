using Queries.Parts.Columns;

namespace Queries.Parts.Clauses
{
    public interface IClause<T> where T : IColumn
    {
        T Column { get; set; }
        ClauseOperator Operator{ get; set; }
        ColumnBase Constraint { get; set; }
    }
}