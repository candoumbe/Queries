using Queries.Core.Builders;

namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// a <see cref="Builders.SelectQuery"/> that can be used as 
    /// <see cref="IColumn"/> in a <see cref="Builders.SelectQuery"/>.
    /// </summary>
    public class SelectColumn : IAliasable<SelectColumn>, IColumn
    {
        private string _alias;

        /// <summary>
        /// Alias of the column
        /// </summary>
        public string Alias => _alias;

        /// <summary>
        /// Sets the column's alias
        /// </summary>
        /// <param name="alias">The new alias</param>
        /// <returns></returns>
        public SelectColumn As(string alias)
        {
            _alias = alias;

            return this;
        }

        /// <summary>
        /// The wrapped query
        /// </summary>
        public SelectQuery SelectQuery { get; set; }

        public static UniqueIdentifierValue UUID() => new UniqueIdentifierValue();
        
        /// <summary>
        /// Performs a deep copy of the current instance.
        /// </summary>
        /// <returns><see cref="SelectColumn"/> that is a deeo copy of the current instance.</returns>
        public IColumn Clone() => new SelectColumn() { SelectQuery = SelectQuery.Clone() }.As(_alias); 
    }
}