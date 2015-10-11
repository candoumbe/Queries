using System;

namespace Queries.Core.Parts.Columns
{
    public abstract class AggregateColumn : IAliasable<AggregateColumn>, IColumn
    {
        public AggregateType Type { get; }

        public IColumn Column { get; }

        protected AggregateColumn(AggregateType aggregate, IColumn column)
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
