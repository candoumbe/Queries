using Queries.Parts.Columns;

namespace Queries.Extensions
{
    public static class LiteralExtensions
    {
        public static StringColumn Literal(this string obj, string alias = null)
        {
            return new StringColumn(obj, alias);
        }

        public static NumericColumn Literal(this int obj, string alias = null)
        {
            return new NumericColumn(obj, alias);
        }

        public static NumericColumn Literal(this float obj, string alias = null)
        {
            return new NumericColumn(obj, alias);
        }

        public static NumericColumn Literal(this double obj, string alias = null)
        {
            return new NumericColumn(obj, alias);
        }
    }
}