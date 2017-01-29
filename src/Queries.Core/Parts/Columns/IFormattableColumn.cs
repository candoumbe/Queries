namespace Queries.Core.Parts.Columns
{
    public interface IFormattableColumn<out T> where T : IColumn
    {
       T Format(string format);
    }
}
