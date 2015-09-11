using System;

namespace Queries.Parts.Columns
{
    public abstract class AggregateColumn : IColumn, IAliasable
    {
        public AggregateType Type { get; set; }

        public FieldColumn Column { get; set; }

        public string Alias { get; set; }


        protected AggregateColumn(AggregateType aggregate, FieldColumn column, string alias = null)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }

            Type = aggregate;
            Column = column;
            Alias = alias ?? String.Empty;
        }
    }
}
