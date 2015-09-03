using Queries.Parts.Columns;

namespace Queries.Builders.Fluent
{
    public interface ISqlSelect
    {
        ISqlFrom Select(IColumn column, params IColumn[] columns);
    }
}
