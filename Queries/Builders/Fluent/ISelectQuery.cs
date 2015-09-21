using Queries.Parts;

namespace Queries.Builders.Fluent
{
    public interface ISelectQuery<T>
    {
        IFromQuery<T> Limit(int limit);
        
        IFromQuery<T> From(params ITable[] tables);

        IFromQuery<T> From(params string[] tables);

        IFromQuery<T> From(SelectTable select);

        IFromQuery<T> From(string tableName);
    }
}