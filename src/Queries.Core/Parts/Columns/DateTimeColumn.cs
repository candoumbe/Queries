using System;

namespace Queries.Core.Parts.Columns
{
    public class DateTimeColumn : LiteralColumn, IFormattableColumn<DateTimeColumn>
    {
        public string StringFormat { get; private set; }


        /// <summary>
        /// Builds a new <see cref="DateTimeColumn"/> instance.
        /// </summary>
        /// <param name="value">value of the column</param>
        public DateTimeColumn(DateTime value) : this(value, "yyyy-MM-dd")
        {}

        /// <summary>
        /// Builds a new <see cref="DateTimeColumn"/> instance.
        /// </summary>
        /// <param name="value">value of the column</param>
        /// <param name="stringFormat">Format of the column</param>
        public DateTimeColumn(DateTime value, string stringFormat) : base(value)
        {
            StringFormat = stringFormat;
        }

        /// <summary>
        /// Specifies an output format
        /// </summary>
        /// <param name="format">format to apply</param>
        /// <returns></returns>
        public DateTimeColumn Format(string format)
        {
            //TODO check that format conforms to RFC if not throw a exception
            StringFormat = format;
       
            return this;
        }
    }
}
