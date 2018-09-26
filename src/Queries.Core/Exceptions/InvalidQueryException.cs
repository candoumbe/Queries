using System;

namespace Queries.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a invalid query is processed
    /// </summary>
    public class InvalidQueryException : Exception
    {
        public InvalidQueryException() : this(String.Empty)
        {}

        public InvalidQueryException(string message) : base(message)
        {}
    }
}
