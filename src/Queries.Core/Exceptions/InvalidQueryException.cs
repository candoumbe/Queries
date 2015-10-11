using System;

namespace Queries.Core.Exceptions
{
    public class InvalidQueryException : Exception
    {

        public InvalidQueryException() : this(String.Empty)
        {}

        public InvalidQueryException(string message) : base(message)
        {}
    }
}
