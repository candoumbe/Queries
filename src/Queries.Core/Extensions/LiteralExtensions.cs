using System;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Extensions
{
    public static class LiteralExtensions
    {
        public static StringColumn Literal(this string obj) => new StringColumn(obj);

        public static NumericColumn Literal(this int obj) => new NumericColumn(obj);

        public static NumericColumn Literal(this float obj) => new NumericColumn(obj);

        public static NumericColumn Literal(this double obj) => new NumericColumn(obj);

        public static DateTimeColumn Literal(this DateTime value) => new DateTimeColumn(value);
    }
}