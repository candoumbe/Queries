using Queries.Core.Builders;
using System;

namespace Queries.Core.Parts.Columns
{
    public class InsertedValue : IInsertable
    {
        public FieldColumn Column { get; set; }

        public IColumn Value { get; set; }

        public InsertedValue(FieldColumn column, IColumn value)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }
            Column = column;
            Value = value;
        }
    }
}
