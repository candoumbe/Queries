using Queries.Core.Parts;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Sorting;

namespace Queries.Core.Builders.Fluent
{
    public interface IFromQuery<T> : IUnionQuery<T>
    {
        IJoinQuery<T> InnerJoin(Table table, IWhereClause clause);

        IJoinQuery<T> LeftOuterJoin(Table table, IWhereClause clause);

        IJoinQuery<T> RightOuterJoin(Table table, IWhereClause clause);

        IOrderQuery<T> OrderBy(params IOrder[] sorts);

        IWhereQuery<T> Where(IWhereClause clause);

        IWhereQuery<T> Where(IColumn column, ClauseOperator @operator, IColumn constraint);

        IWhereQuery<T> Where(IColumn column, ClauseOperator @operator, string constraint);
    }
}