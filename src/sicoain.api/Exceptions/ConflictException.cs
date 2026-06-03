namespace sicoain.api.Exceptions;

/// <summary>
/// Thrown when an operation fails because of a conflict with existing data
/// (e.g., duplicate email, duplicate username). Maps to HTTP 409 Conflict.
/// </summary>
public class ConflictException : InvalidOperationException
{
    public ConflictException(string message) : base(message) { }

    public ConflictException(string message, Exception innerException) : base(message, innerException) { }

    public ConflictException()
    {
    }
}
