using Queries.Core.Builders;

namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// An instanc of this class represents a <see cref="Builders.SelectQuery"/> that can be used as 
    /// <see cref="IColumn"/> in a <see cref="Builders.SelectQuery"/>.
    /// </summary>
    public class SelectColumn : IAliasable<SelectColumn>, IColumn
    {
        private string _alias;

        public string Alias => _alias;

        public SelectColumn As(string alias)
        {
            _alias = alias;

            return this;
        }

        public SelectQuery SelectQuery { get; set; }

        public static UniqueIdentifierValue UUID() => new UniqueIdentifierValue();
    }
}