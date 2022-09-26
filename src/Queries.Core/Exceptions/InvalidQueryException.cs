using System;

namespace Queries.Core.Exceptions;

/// <summary>
/// Exception thrown when a invalid query is processed
/// </summary>
public class InvalidQueryException : Exception
{
    ///<inheritdoc/>
    public InvalidQueryException() : this(string.Empty)
    {}

    ///<inheritdoc/>
    public InvalidQueryException(string message) : base(message)
    {}

    ///<inheritdoc/>
    public InvalidQueryException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
