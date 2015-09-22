using System;

namespace Queries.Parts.Columns
{
    public abstract class AggregateColumn : IAliasable<AggregateColumn>, IColumn
    {
        public AggregateType Type { get; }

        public FieldColumn Column { get; }

        protected AggregateColumn(AggregateType aggregate, FieldColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            Type = aggregate;
            Column = column;
        }


        private string _alias;

        public string Alias => _alias;

        public AggregateColumn As(string alias)
        {
            _alias = alias;

            return this;
        }


    }
}
