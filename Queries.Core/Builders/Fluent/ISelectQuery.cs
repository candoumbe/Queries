using Queries.Core.Parts;

namespace Queries.Core.Builders.Fluent
{
    public interface ISelectQuery<T>
    {
        ISelectQuery<T> Limit(int limit);
        
        IFromQuery<T> From(params ITable[] tables);

        IFromQuery<T> From(params string[] tables);

        //IFromQuery<T> From(SelectTable select);
        
    }
    
}