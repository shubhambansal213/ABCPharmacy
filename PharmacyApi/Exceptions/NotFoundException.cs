namespace PharmacyApi.Exceptions;

/// <summary>
/// Thrown when a requested entity (e.g. a medicine) does not exist.
/// Translated to an HTTP 404 response by the global exception handling middleware.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
