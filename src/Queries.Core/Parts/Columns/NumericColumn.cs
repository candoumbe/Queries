namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// Column that holds a numeric value.
    /// </summary>
    /// <remarks>
    /// Wraps a numeric value so that it can be used wherever a <see cref="ColumnBase"/> can.
    /// </remarks>
    public class NumericColumn : Literal
    {
        /// <summary>
        /// Builds a new <see cref="NumericColumn"/> instance.
        /// </summary>
        /// <param name="value"></param>
        public NumericColumn(int? value = null) : base(value)
        {}

        /// <summary>
        /// Builds a new <see cref="NumericColumn"/> instance.
        /// </summary>
        /// <param name="value"></param>
        public NumericColumn(float? value = null) : base(value)
        { }

        /// <summary>
        /// Builds a new <see cref="NumericColumn"/> instance.
        /// </summary>
        /// <param name="value"></param>
        public NumericColumn(double? value = null) : base(value)
        { }

        /// <summary>
        /// Builds a new <see cref="NumericColumn"/> instance.
        /// </summary>
        /// <param name="value"></param>
        public NumericColumn(decimal? value = null) : base(value)
        { }

        /// <summary>
        /// Builds a new <see cref="NumericColumn"/> instance.
        /// </summary>
        /// <param name="value"></param>
        public NumericColumn(long? value = null) : base(value)
        { }

        public static implicit operator NumericColumn(int value) => new NumericColumn(value);
    }
}