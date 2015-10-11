using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Queries.Core.Parts.Columns
{
    public class DateTimeColumn : LiteralColumn, IFormattableColumn<DateTimeColumn>
    {
        public string StringFormat { get; private set; }

        public DateTimeColumn(DateTime value) : base(value)
        {}


        public DateTimeColumn Format(string format)
        {
            //todo check that format conforms to RFC if not throw a exception
            StringFormat = format;
       
            return this;
        }
    }
}
