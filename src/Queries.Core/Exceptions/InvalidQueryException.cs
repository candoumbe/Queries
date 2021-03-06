﻿using System;

namespace Queries.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a invalid query is processed
    /// </summary>
    public class InvalidQueryException : Exception
    {
        public InvalidQueryException() : this(string.Empty)
        {}

        public InvalidQueryException(string message) : base(message)
        {}

        public InvalidQueryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
