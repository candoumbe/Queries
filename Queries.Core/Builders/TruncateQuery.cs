namespace Queries.Core.Builders
{
    public class TruncateQuery : IQuery
    {
        /// <summary>
        /// Gets or sets the name of the table to truncate.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }
}