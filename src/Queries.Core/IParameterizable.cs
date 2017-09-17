using Queries.Core.Parts.Clauses;
using System.Collections.Generic;

namespace Queries.Core
{
    /// <summary>
    /// Marker that indicates that the element can have one or more <see cref="Variable"/>
    /// </summary>
    public interface IParameterizable
    {

        bool IsParameterized { get; }

        IEnumerable<Variable> GetParameters();
    }
}