using System;

namespace Queries.Parts.Columns
{
    public abstract class AggregateColumn : IColumn
    {
        public AggregateType Type { get; set; }

        public TableColumn Column { get; set; }


        protected AggregateColumn(AggregateType aggregate, TableColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }

            Type = aggregate;
            Column = column;
        }
    }
}
