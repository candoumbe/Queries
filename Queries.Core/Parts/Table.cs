namespace Queries.Core.Parts
{
    public class Table : INamable, ITable, IAliasable<Table>
    {
        

        /// <summary>
        /// Gets or sets the name of the table in a query
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get;}


        internal Table(string tablename, string alias = null)
        {
            Name = tablename;
            Alias = alias;
        }
        

        /// <summary>
        /// Gets the alias of the table
        /// </summary>
        public string Alias { get; private set; }

        public Table As(string alias)
        {
            Alias = alias;
            return this;
        }
    }
}
