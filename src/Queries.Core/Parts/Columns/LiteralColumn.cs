using Queries.Core.Builders;

namespace Queries.Core.Parts.Columns
{
    public class LiteralColumn : ColumnBase, IAliasable<LiteralColumn>, IInsertable
    {
        public object Value { get; set; }


        public LiteralColumn(object value = null)
        {
            Value = value;
        }

        private string _alias;

        public string Alias => _alias;

        public LiteralColumn As(string alias)
        {
            _alias = alias;

            return this;
        }



    }
}
