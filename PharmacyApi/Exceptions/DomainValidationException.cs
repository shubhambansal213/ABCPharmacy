namespace PharmacyApi.Exceptions;

/// <summary>
/// Thrown when a request violates a business/domain rule (e.g. missing required field,
/// selling more stock than is available). Translated to an HTTP 400 response by the
/// global exception handling middleware.
/// </summary>
public class DomainValidationException : Exception
{
    public DomainValidationException(string message) : base(message)
    {
    }
}
