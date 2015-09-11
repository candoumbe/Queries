using System;

namespace Queries.Parts.Columns
{
    public class LiteralColumn : ColumnBase, IAliasable
    {
        public string Alias { get; set; }

        public object Value { get; set; }


        public LiteralColumn(object value = null, string alias = "")
        {
            Value = value;
            Alias = alias ?? String.Empty;
        }


        public static LiteralColumn From(string value)
        {
            return new LiteralColumn(value);
        }
    }
}
