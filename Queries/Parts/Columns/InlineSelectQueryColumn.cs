using Queries.Builders;

namespace Queries.Parts.Columns
{
    /// <summary>
    /// An instanc of this class represents a <see cref="SelectQuery"/> that can be used as <see cref="IColumn"/> in a <see cref="SelectQuery"/>
    /// </summary>
    public class SelectColumn : IAliasable, IColumn
    {
        public string Alias { get; set; }

        public SelectQuery SelectQuery { get; set; }
    }
}