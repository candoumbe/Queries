using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Queries.Core.Builders;

namespace Queries.Core.Parts.Columns
{
    public class InsertedValue : IInsertable
    {
        public FieldColumn Column { get; set; }

        public LiteralColumn Value { get; set; }

        public InsertedValue(FieldColumn column, LiteralColumn value)
        {
            Column = column;
            Value = value;
        }
    }
}
