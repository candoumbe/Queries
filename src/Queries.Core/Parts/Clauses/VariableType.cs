using System;

namespace Queries.Core.Parts.Clauses
{
    /// <summary>
    /// Types of <see cref="Variable"/>s.
    /// </summary>
    [Flags]
    public enum VariableType : short
    {
        /// <summary>
        /// Variable that holds a string value
        /// </summary>
        String= 0b_000,

        /// <summary>
        /// Variable that can hold a numeric value
        /// </summary>
        Numeric = 0b_001,

        /// <summary>
        /// Variable that can hold a boolean value
        /// </summary>
        Boolean= 0b_010,

        /// <summary>
        /// Variable that can hold a date
        /// </summary>
        Date = 0b_100,

        /// <summary>
        /// Variable that can hold a time value
        /// </summary>
        Time = 0b_101
    }
}