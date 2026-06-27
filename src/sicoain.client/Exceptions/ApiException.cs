namespace sicoain.client.Exceptions;

/// <summary>
/// Exception thrown when the API returns a non-success status code with a
/// meaningful error message in the response body.
/// </summary>
public class ApiException : Exception
{
    public ApiException(string message) : base(message)
    {
    }

    public ApiException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
