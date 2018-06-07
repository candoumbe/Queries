using System;

namespace Queries.Core.Parts.Clauses
{
    /// <summary>
    /// Types of <see cref="Variable"/>s.
    /// </summary>
    [Flags]
    public enum VariableType
    {
        Numeric = 0x1,
        String = 0x2,
        Boolean = 0x4,
        Date= 0x8
    }
}