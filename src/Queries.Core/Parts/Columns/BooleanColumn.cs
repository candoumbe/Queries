namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// Column that contains a <see cref="bool"/> value.
    /// </summary>
    public class BooleanColumn : Literal
    { 
        public BooleanColumn(bool value) : base(value)
        { }

    }
}