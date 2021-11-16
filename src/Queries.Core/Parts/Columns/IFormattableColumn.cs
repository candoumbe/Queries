namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// Classes that implement this interface allow to customize the way their content are rendered.
    /// </summary>
    /// <typeparam name="T">Type of the column which content will be formatted.</typeparam>
    public interface IFormattableColumn<out T> where T : IColumn
    {
        /// <summary>
        /// A format to apply to a column
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
       T Format(string format);
    }
}
