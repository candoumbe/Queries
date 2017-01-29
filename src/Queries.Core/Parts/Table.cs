namespace Queries.Core.Parts
{


    public class Table : INamable, ITable, IAliasable<Table>
    {
        /// <summary>
        /// Gets the name of the table in a query
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get;}

        /// <summary>
        /// Builds a new <see cref="Table"/> instance.
        /// </summary>
        /// <param name="tablename">Name of the table</param>
        /// <param name="alias">alias of the table</param>
        internal Table(string tablename, string alias = null)
        {
            Name = tablename;
            Alias = alias;
        }
        

        /// <summary>
        /// Gets the alias of the table
        /// </summary>
        public string Alias { get; internal set; }

        public Table As(string alias)
        {
            Alias = alias;
            return this;
        }
    }
}
