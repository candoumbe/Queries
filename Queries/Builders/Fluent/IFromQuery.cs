using Queries.Parts;
using Queries.Parts.Clauses;
using Queries.Parts.Sorting;

namespace Queries.Builders.Fluent
{
    public interface IFromQuery<T> : IBuildableQuery<T>
    {
       
        IJoinQuery<T> InnerJoin(Table table, IWhereClause clause);

        IJoinQuery<T> LeftOuterJoin(Table table, IWhereClause clause);

        IJoinQuery<T> RightOuterJoin(Table table, IWhereClause clause);

        ISortQuery<T> OrderBy(params ISort[] sorts);

        IWhereQuery<T> Where(IWhereClause clause);
    }
}