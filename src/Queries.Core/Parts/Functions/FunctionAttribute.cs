using System;

namespace Queries.Core.Parts.Functions
{
    /// <summary>
    /// Marks a class as a Function
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class FunctionAttribute : Attribute
    {        
    }
}
