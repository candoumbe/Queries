using Queries.Core.Parts.Columns;

namespace System
{
    /// <summary>
    /// Extension methods for converting primitive types to their <see cref="IColumn"/> equivalent.
    /// </summary>
    public static class LiteralExtensions
    {
        /// <summary>
        /// Converts a <see cref="string"/> to <see cref="StringColumn"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringColumn Literal(this string value) => new StringColumn(value);

        /// <summary>
        /// Converts a <see cref="string"/> to <see cref="StringColumn"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BooleanColumn Literal(this bool value) => new BooleanColumn(value);

        /// <summary>
        /// Converts an <see cref="int"/> to <see cref="NumericColumn"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static NumericColumn Literal(this int value) => new NumericColumn(value);

        /// <summary>
        /// Converts an <see cref="float"/> to <see cref="NumericColumn"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static NumericColumn Literal(this float value) => new NumericColumn(value);

        /// <summary>
        /// Converts an <see cref="double"/> to <see cref="NumericColumn"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static NumericColumn Literal(this double value) => new NumericColumn(value);

        /// <summary>
        /// Converts an <see cref="long"/> to <see cref="NumericColumn"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static NumericColumn Literal(this long value) => new NumericColumn(value);

        /// <summary>
        /// Converts an <see cref="decimal"/> to <see cref="NumericColumn"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static NumericColumn Literal(this decimal value) => new NumericColumn(value);

        /// <summary>
        /// Converts an <see cref="DateTime"/> to <see cref="DateTimeColumn"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format">format to use when converting <paramref name="value"/> to its string representation</param>
        /// <returns></returns>
        public static DateTimeColumn Literal(this DateTime value, string format = default) => format == default
            ? new DateTimeColumn(value)
            : new DateTimeColumn(value, format);

#if NET6_0_OR_GREATER
        /// <summary>
        /// Converts an <see cref="DateOnly"/> to <see cref="DateColumn"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format">format to use when converting <paramref name="value"/> to its string representation</param>
        /// <returns></returns>
        public static DateColumn Literal(this DateOnly value, string format = default) => format == default
            ? new DateColumn(value)
            : new DateColumn(value, format);
#endif
    }
}