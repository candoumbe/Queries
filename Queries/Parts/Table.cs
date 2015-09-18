namespace Queries.Parts
{
    public class Table : INamable, ITable
    {
        /// <summary>
        /// Gets or sets the alias of the table
        /// </summary>
        /// <value>
        /// The alias.
        /// </value>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the name of the table in a query
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }
}
