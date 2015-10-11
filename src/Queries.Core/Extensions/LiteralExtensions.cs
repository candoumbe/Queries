using System;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Extensions
{
    public static class LiteralExtensions
    {
        public static StringColumn Literal(this string obj)
        {
            return new StringColumn(obj);
        }

        public static NumericColumn Literal(this int obj)
        {
            return new NumericColumn(obj);
        }

        public static NumericColumn Literal(this float obj)
        {
            return new NumericColumn(obj);
        }

        public static NumericColumn Literal(this double obj)
        {
            return new NumericColumn(obj);
        }

        public static DateTimeColumn Literal(this DateTime value)
        {
            return new DateTimeColumn(value);
        }
    }
}