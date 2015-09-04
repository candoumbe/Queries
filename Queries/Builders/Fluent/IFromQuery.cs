using Queries.Parts;

namespace Queries.Builders.Fluent
{
    public interface IFromQuery<T> : IBuildableQuery<T>
    {
       
        IJoinQuery<T> InnerJoin(Table table, IClause clause);

        IJoinQuery<T> LeftOuterJoin(Table table, IClause clause);

        IJoinQuery<T> RightOuterJoin(Table table, IClause clause);

        IWhereQuery<T> Where(IClause clause);
    }
}