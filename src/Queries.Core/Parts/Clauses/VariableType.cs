using System;

namespace Queries.Core.Parts.Clauses
{
    /// <summary>
    /// Types of <see cref="Variable"/>s.
    /// </summary>
    [Flags]
    public enum VariableType : short
    {
        String= 0b_000,
        Numeric = 0b_001,
        Boolean= 0b_010,
        Date = 0b_100
    }
}